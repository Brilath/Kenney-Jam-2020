using System;
using UnityEngine;
using TMPro;

public class UIAddFriend : MonoBehaviour
{
    [SerializeField] private TMP_Text addFriendText;
    [SerializeField] private string displayName;
    public static Action<string> OnAddFriend = delegate { };
    public static Action OnNewFriend = delegate { };

    public void SetAddFriendName(string name)
    {
        displayName = name;
    }
    public void AddFriend()
    {
        OnAddFriend?.Invoke(displayName);
        OnNewFriend?.Invoke();
        addFriendText.SetText("");
    }
}
