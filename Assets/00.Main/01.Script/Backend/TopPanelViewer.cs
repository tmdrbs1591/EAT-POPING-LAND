using UnityEngine;
using TMPro;

public class TopPanelViewer : MonoBehaviour
{
	[SerializeField]
	private	TextMeshProUGUI	textNickname;
	[SerializeField] GameObject nickNameInputPanel;

	public void UpdateNickname()
	{
			if(UserInfo.Data.nickname == null)
		{
			nickNameInputPanel.SetActive(true);
		}

		// 닉네임이 없으면 gamer_id를 출력하고, 닉네임이 있으면 닉네임 출력
		textNickname.text = UserInfo.Data.nickname == null ?
							UserInfo.Data.gamerId : UserInfo.Data.nickname;
	}
}

