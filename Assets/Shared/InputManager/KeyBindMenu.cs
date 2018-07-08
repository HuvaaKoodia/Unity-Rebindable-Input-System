using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyBindMenu : MonoBehaviour
{
    #region variables
    public KeybindPopup keybindPopup;
    
    public KeyBindElement elementPrefab;
    public Transform elementParent;
    public RectTransform[] elementGroups;

    int[] elementGroupIndices;
    List<KeyBindElement> elements;
    KeyBindElement escapeMenuElement;

    #endregion
    #region initialization

    void Awake()
    {
        InputManager.Init();
    }

    void Start()
    {
#if UNITY_ANDROID
        gameObject.SetActive(false);
        return;
#endif

        elements = new List<KeyBindElement>();
        elementGroupIndices = new int[elementGroups.Length];

        foreach (var item in InputManager.keybindsRead)
        {
            var element = Instantiate(elementPrefab, elementParent);
            if (item.group >= 0 && item.group < elementGroups.Length)
            {
                var groupParent = elementGroups[item.group];
                elementGroupIndices[item.group]++;
                element.transform.SetSiblingIndex(groupParent.GetSiblingIndex()+ elementGroupIndices[item.group]);
            }

            element.visualName = item.visualName;
            element.keybindName = item.name;

            element.onBind1ButtonPressed += OnKeyBind1Pressed;
            element.onBind2ButtonPressed += OnKeyBind2Pressed;
            elements.Add(element);

            if (element.keybindName == "EscapeMenu")
            {
                escapeMenuElement = element;
            }
        }
    }

    #endregion

    #region logic

    #endregion

    #region public interface

    public void CheckEscapeMenuBind()
    {
        if (escapeMenuElement && escapeMenuElement.HasNoBind())
            escapeMenuElement.SetBind1(KeyCode.Escape);
    }

    #endregion

    #region private interface
    
    #endregion

    #region events

    void OnKeyBind1Pressed(KeyBindElement element)
    {
        keybindPopup.Open(element,true);
    }

    void OnKeyBind2Pressed(KeyBindElement element)
    {
        keybindPopup.Open(element,false);
    }

    public void OnReset()
    {
        InputManager.ResetKeybinds();
        foreach (var element in elements)
        {
            element.Reset();
        }
    }

    
    #endregion
}