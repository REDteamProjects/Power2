using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
using Windows_Ad_Plugin;
#endif

/// <summary>
/// A separate component for displaying fake ads when the ad is not filled
/// Comply with any and all EULAs and do not abuse this feature
/// </summary>
public class AdFiller : MonoBehaviour
{
#if (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)

    //Ad filler's width and height - in pixels
    public float adFillerWidth = 0.0f;
    public float adFillerHeight = 0.0f;

    //How long we should wait till the adFiller ad is swapped 
    public float swapDelay = 30.0f;

    public UnityEngine.UI.Image buttonImage;

    //Horizontal and Vertical Alignment of adFiller
    //Ideally, these would be properties but that breaks once we build it out
    //So we just tell it that it is not serialized so it wont show up in the editor
    [System.NonSerialized]
    public Windows_Ad_Plugin.Helper.HORIZONTAL_ALIGNMENT horizontalAlignment;
    [System.NonSerialized]
    public Windows_Ad_Plugin.Helper.VERTICAL_ALIGNMENT verticalAlignment;

    //The AdFiller's rectangle for drawing
    public RectTransform adRect;

    //Colour for the image, strange bug where it breaks if we directly activate/deactivate the object
    private Color imageColour;

    //Current ad to show
    int curAd = 0;

    [System.Serializable]
    public class AdFillerEntry
    {
        public Sprite image;
        public string url = "";
    }

    public List<AdFillerEntry> adFillers;

    // Use this for initialization
    void Start()
    {
        Windows_Ad_Plugin.Helper.Instance.OnErrorDelegate += ShowAdFiller;
        Windows_Ad_Plugin.Helper.Instance.OnRefreshedDelegate += HideAdFiller;

        //For some reason, it breaks when we activate/deactivate the button through the script. Within Unity it is fine, once built however it breaks
        //To deal with this we just 'hide' the buttonImage by settings its opacity to 0
        imageColour = buttonImage.color;
        imageColour.a = 0.0f;
        buttonImage.color = imageColour;
    }

    private void ShowAdFiller() 
    {

        //If the adFillers list is empty or if the button gameobject is already shown, we return
         if (adFillers.Count == 0 || buttonImage.color.a == 1.0f)
            return;

        //Change the Button's sprite
        buttonImage.sprite = adFillers[curAd].image;

        //Show the button
        imageColour.a = 1.0f;
        buttonImage.color = imageColour;

        //Increase the current ad
        curAd++;
        //Keep it within bounds
        if (curAd > adFillers.Count - 1)
            curAd = 0;
    }

    /// <summary>
    /// Hides our adFiller
    /// </summary>
    private void HideAdFiller()
    {
        //Hide the button
        imageColour.a = 0.0f;
        buttonImage.color = imageColour;
    }

    /// <summary>
    /// Updates the AdFillers position and size
    /// </summary>
    public void UpdateAdRect()
    {
        if (buttonImage == null)
            return;

        //Set the rectangles position
        Vector2 pos;
        pos.x = GetXConversion(adFillerWidth);
        pos.y = GetYConversion(adFillerHeight);
        adRect.position = pos;

        Vector2 size;
        size.x = adFillerWidth;
        size.y = adFillerHeight;

        //Then its width/height
        adRect.sizeDelta = size;
    }

    /// <summary>
    /// Converts the vertical alignment value, and the passed in height and converts it into screen space
    /// </summary>
    private float GetYConversion(float height)
    {
        float half = height * 0.5f;
        switch (verticalAlignment)
        {
            case Helper.VERTICAL_ALIGNMENT.CENTER:
                return (Screen.height * 0.5f) - half;
            case Helper.VERTICAL_ALIGNMENT.TOP:
                return Screen.height - half;
            case Helper.VERTICAL_ALIGNMENT.BOTTOM:
                return half;
        }

        return 0.0f;
    }

    /// <summary>
    /// Converts the horizontal alignment value, and the passed in height and converts it into screen space
    /// </summary>
    private float GetXConversion(float width)
    {
        float half = width * 0.5f;
        switch (horizontalAlignment)
        {
            case Helper.HORIZONTAL_ALIGNMENT.CENTER:
                return (Screen.width * 0.5f);
            case Helper.HORIZONTAL_ALIGNMENT.LEFT:
                return half;
            case Helper.HORIZONTAL_ALIGNMENT.RIGHT:
                return Screen.width - half;

        }

        return 0.0f;
    }

    /// <summary>
    /// Handles the UI Button's click
    /// Launches a web browser
    /// </summary>
    public void ClickHandler()
    {
        if (adFillers.Count > 0 && buttonImage.color.a > 0.0f)
            if(adFillers[curAd].url != "")
                Application.OpenURL(adFillers[curAd].url);
    }
#endif
}
