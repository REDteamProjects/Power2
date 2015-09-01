import os
from sys import argv
from mod_pbxproj import XcodeProject

#path = "/Users/jesudaslobo/VservAdUnitySample/VservUnityResources"
path = argv[1]
#fileToAddPath = "/Users/jesudaslobo/VservAdUnitySample/Assets/WebPlayerTemplates"
fileToAddPath = argv[2]

#Adding Required Frameworks for Ad SDK
#if framework is optional, add `weak=True`

project = XcodeProject.Load(path +'/Unity-iPhone.xcodeproj/project.pbxproj')
project.add_file_if_doesnt_exist('System/Library/Frameworks/CoreTelephony.framework', tree='SDKROOT')
project.add_file_if_doesnt_exist('System/Library/Frameworks/MobileCoreServices.framework', tree='SDKROOT')
project.add_file_if_doesnt_exist('System/Library/Frameworks/StoreKit.framework', tree='SDKROOT')
project.add_file_if_doesnt_exist('System/Library/Frameworks/Social.framework', tree='SDKROOT', weak=True)
project.add_file_if_doesnt_exist('System/Library/Frameworks/EventKit.framework', tree='SDKROOT')
project.add_file_if_doesnt_exist('System/Library/Frameworks/EventKitUI.framework', tree='SDKROOT')
project.add_file_if_doesnt_exist('System/Library/Frameworks/AdSupport.framework', tree='SDKROOT')
project.add_file_if_doesnt_exist('System/Library/Frameworks/MessageUI.framework', tree='SDKROOT')

project.add_file_if_doesnt_exist('usr/lib/libsqlite3.dylib', tree='SDKROOT')


#Adding the Vserv Ad SDK
files_in_dir = os.listdir(fileToAddPath)

for f in files_in_dir:
    if not f.startswith('.'): #exclude .DS_STORE on mac
        print (f)
        
    pathname = os.path.join(fileToAddPath, f)
    fileName, fileExtension = os.path.splitext(pathname)
    
    if not fileExtension == '.meta': #skip .meta file
        if os.path.isfile(pathname):
            print "is file : " + pathname
            project.add_file_if_doesnt_exist(pathname)
            
        if os.path.isdir(pathname):
            print "is dir : " + pathname
            project.add_folder(pathname, excludes=["^.*\.meta$"])
            
#Adding Linker Flags
project.add_other_ldflags('-ObjC')

# project.add_other_buildsetting('GCC_ENABLE_OBJC_EXCEPTIONS', 'YES')

#Save changes to Xcode project file
if project.modified:
    project.backup()
    project.saveFormat3_2()
