using Com.Tencent.Chat.UIKit.Unity;
using com.tencent.imsdk.unity;
using com.tencent.imsdk.unity.types;
using com.tencent.imsdk.unity.enums;
using UnityEngine;
using com.tencent.imsdk.unity.callback;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Tencent.Chat.UIKit.Unity
{
  public class Core
  {
    public static string sdkappid { get; set; }
    public static string userid { get; set; }
    public static string usersig { get; set; }
    public static int currentStickerIndex { get; private set; } = 0;
    public static string currentConvID { get; private set; }
    public static TIMConvType currentConvType { get; private set; }

    public static Dictionary<string, MessageListObject> convMessageMap { get; private set; } = new Dictionary<string, MessageListObject>();
    public static Dictionary<string, int> convMessageIndex { get; private set; } = new Dictionary<string, int>();
    public delegate void OnChanged(params string[] args);
    public delegate void Callback();

    public static Callback OnCloseConversation;
    public static OnChanged OnCurrentConvChanged;
    public static OnChanged OnCurrentStickerIndexChanged;
    public static OnChanged OnMsgListChanged;
    public static OnChanged OnStickerPackageChanged;
    public static OnChanged OnConvChanged;
    public static Utils.Callback OnLogin;
    public static void SetCurrentConv(string convID, TIMConvType convType)
    {
      currentConvID = convID;
      currentConvType = convType;
      if (OnCurrentConvChanged != null)
      {
        OnCurrentConvChanged();
      }
    }

    public static void SetCurrentStickerIndex(int index)
    {
      currentStickerIndex = index;
      if (OnCurrentStickerIndexChanged != null)
      {
        OnCurrentStickerIndexChanged();
      }
    }

    public static List<StickerPackage> stickers { get; private set; } = new List<StickerPackage>();

    public static void SetStickerPackageList(List<StickerPackage> list)
    {
      stickers = list;
      if (OnStickerPackageChanged != null)
      {
        OnStickerPackageChanged();
      }
    }

    public static void SetConfig(string sdkappId, string userId = null, string userSig = null)
    {
      sdkappid = sdkappId;
      userid = userId;
      usersig = userSig;
    }

    // public static void AddStickerPackage(StickerPackage pack, int? index)
    // {

    // }

    public static void SetMessageList(string convID, List<Message> msgList, bool finished = false)
    {
      if (convMessageMap.TryGetValue(convID, out MessageListObject msgListObj))
      {
        var msgListFiltered = Utils.FilterMsgList(msgListObj.list, msgList);

        if (Utils.IsMsgListBefore(msgListFiltered, msgListObj.list))
        {
          // New messages
          msgListObj.list.InsertRange(0, msgListFiltered);
          msgListObj.isPrevMessage = false;
        }
        else
        {
          // Coming message list is before the original message list
          // Earlier messages
          msgListObj.list.AddRange(msgListFiltered);
          msgListObj.isPrevMessage = true;
        }
        msgListObj.finished = finished;
        msgListObj.loading = false;
      }
      else
      {
        convMessageMap.Add(convID, new MessageListObject
        {
          list = msgList,
          finished = finished,
          loading = false
        });

      }

      if (msgList.Count < 1)
      {
        return;
      }

      if (OnMsgListChanged != null)
      {
        OnMsgListChanged(convID);
      }
    }

    public static void Init()
    {
      SdkConfig sdkConfig = new SdkConfig();

      sdkConfig.sdk_config_config_file_path = Application.persistentDataPath + "/TIM-Config";

      sdkConfig.sdk_config_log_file_path = Application.persistentDataPath + "/TIM-Log";

      if (sdkappid == "")
      {
        Debug.LogError("Input sdkappid first");
        return;
      }

      TencentIMSDK.Init(long.Parse(sdkappid), sdkConfig, true);
      TencentIMSDK.AddRecvNewMsgCallback(Utils.HandleNewMsgReceived);
      TencentIMSDK.SetConvEventCallback((TIMConvEvent conv_event, List<ConvInfo> conv_list, string user_data) =>
      {
        if (OnConvChanged != null)
        {
          OnConvChanged("0");
        }
      });

      ExperimentalAPIReqeustParam param = new ExperimentalAPIReqeustParam();

      param.request_internal_operation = TIMInternalOperation.internal_operation_set_ui_platform.ToString();

      param.request_set_ui_platform_param = 6;
      TIMResult res = TencentIMSDK.CallExperimentalAPI(param, (int code, string desc, string data, string user_data) =>
    {
      Debug.Log("Tencent Cloud IM UIKit add config success ." + data);
    });
    }

    public static void Login(Utils.Callback cb = null)
    {
      if (cb != null)
      {
        OnLogin += cb;
      }
      TencentIMSDK.Login(userid, usersig, Utils.HandleStringCallback((string[] args) =>
      {
        if (OnLogin != null)
        {
          OnLogin(args);
          OnLogin = null;
        }
      }));
    }

    public static void Logout(Utils.Callback cb = null)
    {
      TencentIMSDK.Logout(Utils.HandleStringCallback((string[] parameters) =>
      {
        if (cb != null)
        {
          cb(parameters);
        }

        if (Utils.IsCallbackLegit(parameters[0]))
        {
          Reset();
        }
      }));
    }

    public static void Reset()
    {
      currentStickerIndex = 0;
      currentConvID = "";
      currentConvType = TIMConvType.kTIMConv_Invalid;
      convMessageMap = new Dictionary<string, MessageListObject>();
      convMessageIndex = new Dictionary<string, int>();
    }

    public static void Uninit()
    {
      TencentIMSDK.Uninit();
    }

    public static void LoginIfNot(Utils.Callback callback = null)
    {
      try
      {
        if (callback != null)
        {
          OnLogin += callback;
        }
        if (sdkappid == null || userid == null || usersig == null)
        {
          return;
        }

        StringBuilder sb = new StringBuilder(128);
        TencentIMSDK.GetLoginUserID(sb);
        if (string.IsNullOrEmpty(sb.ToString()))
        {
          Init();
          Login();
        }
        else if (OnLogin != null)
        {
          OnLogin("0");
          OnLogin = null;
        }
      }
      catch (Exception e)
      {
        Debug.LogError("Login Error: " + e.ToString());
      }
    }

    public static void MsgGetMsgList(string convID, TIMConvType convType, Utils.Callback callback, Message lastMsg = null)
    {

      if (convMessageMap.TryGetValue(convID, out MessageListObject msgListObj))
      {
        if (msgListObj.loading || msgListObj.finished)
        {
          return;
        }
        msgListObj.loading = true;
      }

      var getMessageListParam = new MsgGetMsgListParam
      {
        msg_getmsglist_param_count = 20,
      };

      if (lastMsg != null)
      {
        getMessageListParam.msg_getmsglist_param_last_msg = lastMsg;
      }

      TencentIMSDK.MsgGetMsgList(convID, convType, getMessageListParam, Utils.HandleStringCallback(callback));
    }

    public static void CloseChat()
    {
      if (OnCloseConversation != null)
      {
        OnCloseConversation();
      }
    }
  }

  public class MessageListObject
  {
    public bool loading = false;
    public float index = 0;
    public bool finished = false;
    public List<Message> list;
    // 当前是否拉取之前的历史消息
    public bool isPrevMessage = false;
  }
}