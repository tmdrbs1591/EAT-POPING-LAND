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

		// �г����� ������ gamer_id�� ����ϰ�, �г����� ������ �г��� ���
		textNickname.text = UserInfo.Data.nickname == null ?
							UserInfo.Data.gamerId : UserInfo.Data.nickname;
	}
}

