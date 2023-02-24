using Com.Tencent.Chat.UIKit.Unity;
using com.tencent.imsdk.unity;
using com.tencent.imsdk.unity.types;
using com.tencent.imsdk.unity.enums;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections.Generic;

namespace Com.Tencent.Imsdk.Unity.UIKit
{
  public class Conversation : MonoBehaviour
  {
    public GameObject conversationItem;
    public Button closeButton;
    void Start()
    {
      // 当前选择的会话变化
      Core.OnCurrentConvChanged += OnCurrentConvChanged;
      // 会话有变更
      Core.OnConvChanged += HandleGetConvList;
      Core.LoginIfNot(HandleGetConvList);
      closeButton.onClick.AddListener(HandleCloseConversation);
    }

    private void HandleCloseConversation()
    {
      Core.CloseChat();
    }

    private void HandleGetConvList(params string[] args)
    {
      if (Utils.IsCallbackLegit(args[0]))
      {
        GetConversationList();
      }
    }

    private void GetConversationList()
    {
      TencentIMSDK.ConvGetConvList(Utils.HandleStringCallback(GenConvItem));
    }

    private void OnCurrentConvChanged(params string[] args)
    {
      if (string.IsNullOrEmpty(Core.currentConvID))
      {
        return;
      }

      // 更改选中会话样式
      var parent = GameObject.Find("ConversationList");
      foreach (Transform child in parent.transform)
      {
        if (child.GetComponentInChildren<Text>().name == Core.currentConvID)
        {
          child.GetComponent<Image>().color = new Color32(22, 74, 165, 100);
        }
        else
        {
          child.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        }
      }
    }

    // 渲染会话项目
    private void GenConvItem(params string[] args)
    {
      if (Utils.IsCallbackLegit(args[0]))
      {
        var convList = Utils.FromJson<List<ConvInfo>>(args[2]);
        var parent = GameObject.Find("ConversationList");
        if (parent == null)
        {
          return;
        }

        foreach (Transform child in parent.transform)
        {
          GameObject.Destroy(child.gameObject);
        }
        foreach (var convInfo in convList)
        {
          var obj = Instantiate(conversationItem, parent.transform);
          obj.SetActive(true);
          obj.GetComponentInChildren<Text>().text = convInfo.conv_show_name;
          obj.GetComponentInChildren<Text>().name = convInfo.conv_id;
          if (obj.GetComponentInChildren<Text>().name == Core.currentConvID)
          {
            obj.GetComponent<Image>().color = new Color32(22, 74, 165, 100);
          }
          else
          {
            obj.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
          }
          obj.GetComponent<Button>().onClick.AddListener(() =>
        {
          Core.SetCurrentConv(convInfo.conv_id, convInfo.conv_type);
        });
        }
      }
    }
  }
}