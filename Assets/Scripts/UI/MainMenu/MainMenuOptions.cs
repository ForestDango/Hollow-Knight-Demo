using System;
using UnityEngine.UI;

public class MainMenuOptions : PreselectOption
{
    public MenuButton startButton;
    public MenuButton optionsButton;

    public MenuButton quitButton;

    public void ConfigureNavigation()
    {
	Navigation navigation = optionsButton.navigation;
	Navigation navigation2 = quitButton.navigation;
	navigation.selectOnDown = quitButton;
	navigation2.selectOnUp = optionsButton;
    }
}
