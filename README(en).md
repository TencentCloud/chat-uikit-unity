<br>

<p align="center">
  <a href="https://www.tencentcloud.com/products/im">
    <img src="https://qcloudimg.tencent-cloud.cn/raw/686d00a64abccadb76ade1ecff4b0ce4.png" width="288px" alt="Tencent Chat Logo" />
  </a>
</p>

<h1 align="center">Tencent Cloud Unity UIKit</h1>

<p align="center">
  Use Tencent Cloud Chat Unity UIKit to quickly build game chat scenarios
</p>
<br>

<p align="center">
    <img src="https://qcloudimg.tencent-cloud.cn/raw/16b6b41518e46ca431e7384014810980.png">
</p>
<br>

## Introduction

`Tencent Cloud Chat Unity UIKit` is a library of business UI components for game scenarios implemented based on [Tencent Cloud Chat Chat SDK](https://github.com/TencentCloud/chat-sdk-unity). Currently it includes features like: `Conversation` and `Chat` components, sending and receiving text messages, sending and receiving sticker messages, customizing sticker.


## Quick Start

### Pre-requisites

- [Signed up](https://www.tencentcloud.com/document/product/378/17985?from=unity) for a Tencent Cloud account and completed [identity verification](https://www.tencentcloud.com/document/product/378/3629?from=unity).
- Created a chat application as instructed in [Creating and Upgrading an Application](https://www.tencentcloud.com/document/product/1047/34577?from=unity) and recorded the SDKAppID.
- Select [Auxiliary Tools](https://console.tencentcloud.com/im-detail/tool-usersig?from=unity) > UserSig Generation and Verification on the left sidebar. Generate "UserID" and the corresponding "UserSig", and copy the "key" information. [Refer to here](https://www.tencentcloud.com/document/product/1047/34580?from=unity#usersig-generation-and-verification).

### Import Package

-  1. Create/start an existing Unity project.
   2. In the `Packages/manifest.json` file, under dependencies, add:
    ```json
      {
        "dependencies":{
          "com.tencent.imsdk.unity.uikit":"https://github.com/TencentCloud/chat-uikit-unity"
        }
      }
    ```
- Copy all files from the [Project](https://github.com/TencentCloud/chat-uikit-unity) `Assets` directory to the local `Assets/` folder

### Step 1: Initialize and log in to Chat

There are two ways to initialize and log in to Chat:

- **Outside Component**: The entire application is initialized and logged in once.
- **Inside Component**: Parameters are passed internally to the component by means of configuration. It is recommended that you use internal login, as UIKit already binds the appropriate event callbacks for you, including event for receiving new messages and event for conversation list updates.

#### Outside Component

Initialize Chat in the Unity project you created, noting that Chat applications only need to be initialized once. You can skip this step if you are integrating in an existing Chat project.

```c#
public static void Init() {
        int sdkappid = 0; // Get the application SDKAppID from the Chat console.
        SdkConfig sdkConfig = new SdkConfig();

        sdkConfig.sdk_config_config_file_path = Application.persistentDataPath + "/TIM-Config";

        sdkConfig.sdk_config_log_file_path = Application.persistentDataPath + "/TIM-Log"; // Set local log address

        TIMResult res = TencentIMSDK.Init(long.Parse(sdkappid), sdkConfig);
}

public static void Login() {
  if (userid == "" || user_sig == "")
  {
      return;
  }
  TIMResult res = TencentIMSDK.Login(userid, user_sig, (int code, string desc, string json_param, string user_data)=>{
    // Handle login callback logic
  });
```

##### Inside Component(Recommended)

You can also pass `SDKAppID`, `UserSig`, `UserID` into the component for Chat initialization and login via configuration。

```c#
using com.tencent.imsdk.unity.uikit;

public static void Init() {
  Core.SetConfig(sdkappid, userId, sdkUserSig);
  Core.Init();
  Core.Login();
}
```

### Step 2: Import Stickers (optional)

Tencent Cloud Chat Unity UIKit currently provides sending and rendering of text and sticker packs. You need to import the sticker packs in advance.

1. Import the used stickers in the `Assets/Resources` folder
    <p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/ea516e9b19793282a49c81570d17c559.png">
    </p>
2. Change the `Texture Type` of the image to `Sprite (2D and UI)` and modify `Pixels Per Unit` according to the image size
    <p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/d5cad0548b08be9413a7e3a92ed0c956.png">
    </p>
   1. Define the corresponding sticker data
   ```c#
      // Generate a list of sticker packs, StickerPackage for a set of sticker packs
      List<StickerPackage> stickers = new List<StickerPackage> {
      new StickerPackage {
        name = "4350",
        baseUrl = "custom_sticker_resource/4350", //Relative paths within folder Resource
        menuItem = new StickerItem { // Sticker bar sticker items
          name = "menu@2x",
          index = 0,
        },
        stickerList = new List<StickerItem> { // Sticker item list
          new StickerItem {
          name = "menu@2x",
          index = 0 // Sticker index
        },
        }
      }
    };
   ```
3. Register Stickers to UIKit
   ```c#
   using com.tencent.imsdk.unity.uikit;

      Core.SetStickerPackageList(Config.stickers);
   ```


### Step 3: Using Conversation and Chat prefabs

Reference the prefabs `ChatPanel` and `ConversationPanel` in the scene and adjust the layout accordingly
    <p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/99991ae822e1855ee65d4beb1058b502.png">
    </p>

Execute `SetConfig`, `Init` and `Login` in Script

```c#
    using com.tencent.imsdk.unity.uikit;

      Core.SetConfig(sdkappid, userid, sdkusersig); // Set sdk account information
      Core.Init();
      Core.Login((params string[] args) => {
        // Handling Login callbacks
      });
```


## Prefabs Description

You can modify the style of your project by modifying the style of the Prefabs, which are currently available as follows.

1. ChatPanel: Include message display area `MessageContent`, Input operation area `ActionPanel` and the sticker area `OverlayPanel`
    <p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/adcb0192f03a88689addb13e9bebadbf.png">
    </p>
2. ConversationPanel: Include conversation list header area `ConversationHeaderPanel` and conversation list display area `ConversationListPanel`
    <p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/e10e0c9d36293dcdaab80674d5756faf.png">
    </p>
3. MessageItem, MessageItemSelf, StickerMessageItem, StickerMessageItemSelf, TimeStamp are message items. They represent: text message type sent by others, text message type sent by yourself, sticker message type sent by others, sticker message type sent by yourself and timestamp message type
4. ConversationItem is conversation list item
5. MenuItem, StickerItem represents sticker bar menu item and sticker list item

## API Documents

### SetConfig

Pass Config information before Init, including `sdkappid`, `userid` and `usersig`.

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.SetConfig(sdkappid, userid, usersig);
```

### Init

The SDK is initialized using the Init method provided by UIKit, which automatically binds the `AddRecvNewMsgCallback` and `SetConvEventCallback` callbacks.

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.Init();
```

### SetStickerPackageList

Set the list of stickers with `SetStickerPackageList`.

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.SetStickerPackageList(Config.stickers);
```

### Login

Login to the account via `Login` and execute the bound callback function when the login is complete.

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.Login((params string[] args) => {
        // Handling Login callbacks
      });
```

### SetMessageList

Add a list of messages for a conversation, process them and merge them into the current conversation message dictionary, and trigger the `OnMsgListChanged` event.

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.SetMessageList(currentConvID, newMsgList, isFinished);
```

### SetCurrentConv

Set the currently selected conversation and trigger the `OnCurrentConvChanged` event.

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.SetMessageList(convID, convType);
```

### SetCurrentStickerIndex

Set the currently selected sticker group and trigger the `OnCurrentStickerIndexChanged` event.

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.SetMessageList(stickerIndex);
```

### Logout

Log out and reset chat data.

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.Logout((string[] parameters) => {
        // Handling Logout Callbacks
      });
```

## TencentIMSDK

The [Unity TencentIMSDK](https://www.tencentcloud.com/document/product/1047/41656) provides comprehensive instant messaging capabilities based on the Unity platform. You can use `TencentIMSDK` to get other instant messaging related features. For example, you can use `TencentIMSDK` to get user information

```c#
using com.tencent.imsdk.unity;

    // Get personal information
    FriendShipGetProfileListParam param = new FriendShipGetProfileListParam
    {
      friendship_getprofilelist_param_identifier_array = new List<string>
      {
        "self_userid"
      }
    };

    TIMResult res = TencentIMSDK.ProfileGetUserProfileList(param, (int code, string desc, List<UserProfile> profile, string user_data)=>{
      // Handling asynchronous logic
    });
```

## Contact Us

Please do not hesitate to contact us in the following place, if you have any further questions or tend to learn more about the use cases.

- Telegram Group: https://t.me/+1doS9AUBmndhNGNl
- WhatsApp Group: https://chat.whatsapp.com/Gfbxk7rQBqc8Rz4pzzP27A
- QQ Group: 764231117, chat in Chinese

Our Website: https://www.tencentcloud.com/products/im?from=unity