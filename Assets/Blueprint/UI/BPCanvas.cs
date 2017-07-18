using UnityEngine;

public class BPCanvas : MonoBehaviour {
	public static BPCanvas bpCanvas;
	public TitlePanel titlePanel;
	public SelectMapPanel selectMapPanel;
	public SettingPanel settingPanel;
	public AddMapPanel addMapPanel;
	public LoadingMapPanel loadingMapPanel;
	public PausePanel pausePanel;

	void Awake () {
		bpCanvas = this;
		titlePanel = GetComponentInChildren<TitlePanel> (true);
		selectMapPanel = GetComponentInChildren<SelectMapPanel> (true);
		settingPanel = GetComponentInChildren<SettingPanel> (true);
		addMapPanel = GetComponentInChildren<AddMapPanel> (true);
		loadingMapPanel = GetComponentInChildren<LoadingMapPanel> (true);
		pausePanel = GetComponentInChildren<PausePanel> (true);
	}
}
