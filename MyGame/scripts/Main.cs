using System;
using Godot;

namespace Main
{
    public class Main : Node2D
    {
        private Grid _grid;
        // private GameUI _gameUI;
        private ControlTemplate _settingsControl;
        private ControlTemplate _mainControl;
        private HelpControl _helpControl;
        private Control _gridControl;
        private Tween _tween;
        private int _highscore = -1;
        private Label _highscoreLabel;

        public override void _Ready()
        {
            // _gameUI = GetNode<GameUI>("GridLayer/MainNode/GameUI");
            _gridControl = GetNode<Control>("GridLayer/MainControl/GridControl");
            _settingsControl = GetNode<ControlTemplate>("GridLayer/SettingsControl");
            _mainControl = GetNode<ControlTemplate>("GridLayer/MainControl");
            _helpControl = GetNode<HelpControl>("GridLayer/HelpControl");
            _tween = GetNode<Tween>("MainTween");

            _highscoreLabel = _settingsControl.GetNode<Label>("HighscoreLabel");
            _highscore = SaveManager.LoadHighscore();

            if (_highscore != -1)
            {
                _highscoreLabel.Text = $"Highscore: {_highscore}";
            }


            // PackedScene _gridScene = (PackedScene)ResourceLoader.Load("res://scene/Grid.tscn");
            _grid = Globals.PackedScenes.GridScene.Instance<Grid>();

            int sizeConstraint = (int)GetViewport().GetVisibleRect().Size.x - 200;
            Vector2 cellRatio = new Vector2(1f, 1f);

            _grid.Init(true, new Vector2(4, 6), new Vector2(64, 64) * cellRatio, new Vector2(10, 10), sizeConstraint);
            UpdateGridInfo();

            _gridControl.AddChild(_grid);

            RotateGrid();

            _helpControl.InstanceGrid(new Vector2(4, 6), new Vector2(64, 64), new Vector2(10, 10), cellRatio, sizeConstraint - 50);

        }


        public void _on_GameUI_button_pressed(string buttonName)
        {
            switch (buttonName)
            {
                case "RestartButton":
                    _grid.Restart();
                    break;

                case "SettingsButton":
                    _highscore = SaveManager.LoadHighscore();
                    
                    if (_highscore != -1)
                    {
                        _highscoreLabel.Text = $"Highscore: {_highscore}";
                    }

                    ChangePanel(_settingsControl, _mainControl);
                    break;

                case "HelpButton":
                    ChangePanel(_helpControl, _mainControl);
                    _helpControl.StartHelpTween();
                    break;
            }
        }
        public void _on_SettingsControl_button_pressed(string buttonName)
        {
            switch (buttonName)
            {
                case "SoundOnButton":
                    break;

                case "MusicButton":
                    break;

                case "BackButton":
                    ChangePanel(_mainControl, _settingsControl);
                    break;
            }
        }
        public void _on_HelpControl_button_pressed(string buttonName)
        {
            switch (buttonName)
            {
                case "BackButton":
                    _helpControl.StopHelpTween();
                    ChangePanel(_mainControl, _helpControl);
                    break;
            }
        }


        public void RotateGrid()
        {
            _grid.Rotation = Mathf.Pi;
        }

        private void UpdateGridInfo()
        {
            Globals.GridInfo.UpdateGridInfo(_grid.GridSize, _grid.CellSize, _grid.CellBorder, _grid.Offset);
        }

        private async void ChangePanel(ControlTemplate controlIn, ControlTemplate controlOut)
        {
            TweenManager.ChangePanelSwap(_tween, controlOut, controlIn);

            controlOut.Visible = true;
            controlIn.Visible = true;

            TweenManager.Start(_tween);

            await ToSignal(_tween, "tween_all_completed");

            controlOut.UpdateState();
            controlIn.UpdateState();
        }


    }
}