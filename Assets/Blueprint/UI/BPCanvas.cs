﻿using UnityEngine;

public class BPCanvas : MonoBehaviour {
	public static TitlePanel titlePanel;
	public static SelectMapPanel selectMapPanel;
	public static SettingPanel settingPanel;
	public static AddMapPanel addMapPanel;
	public static LoadingMapPanel loadingMapPanel;
	public static PausePanel pausePanel;
	public static TitleBackPanel titleBackPanel;

	void Awake () {
		titlePanel = GetComponentInChildren<TitlePanel> (true);
		selectMapPanel = GetComponentInChildren<SelectMapPanel> (true);
		settingPanel = GetComponentInChildren<SettingPanel> (true);
		addMapPanel = GetComponentInChildren<AddMapPanel> (true);
		loadingMapPanel = GetComponentInChildren<LoadingMapPanel> (true);
		pausePanel = GetComponentInChildren<PausePanel> (true);
		titleBackPanel = GetComponentInChildren<TitleBackPanel> (true);
	}
}
