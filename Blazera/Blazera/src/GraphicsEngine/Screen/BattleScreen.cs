﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;

namespace Blazera
{
    public class BattleScreen : Screen
    {
        CombatMap Map;
        FpsLabel Fps = new FpsLabel();

        public BattleScreen(RenderWindow window) :
            base(window)
        {
            Type = ScreenType.BattleScreen;
        }

        public override void FirstInit()
        {
            base.FirstInit();

            Map = new CombatMap(GameScreen.GetCurrentMap());

            Gui.AddWidget(Fps);

            Gui.AddGameWidget(Map.Combat.MainMenu);
            Gui.AddGameWidget(Map.Combat.ActionMenu);
            Gui.AddGameWidget(Map.Combat.SpellMenu);
            Gui.AddGameWidget(Map.Combat.Cursor);

            Map.Combat.InfoPanel.SetLocation(GameWidget.ELocation.BottomRight);
            Gui.AddGameWidget(Map.Combat.InfoPanel);

            Map.Combat.MainMenu.BackgroundBottom = Gui.Bottom;
            Map.Combat.InfoPanel.BackgroundRight = Gui.Right;
            Map.Combat.InfoPanel.BackgroundBottom = Gui.Bottom;

            Gui.SetFirst(Map.Combat.Cursor);
        }

        public override ScreenType Run(Time dt)
        {
            UpdateView(dt);

            Map.Update(dt);
            Map.Draw(Window);

            return base.Run(dt);
        }

        public override bool HandleEvent(BlzEvent evt)
        {
            if (base.HandleEvent(evt))
                return true;

            switch (evt.Type)
            {
                case SFML.Window.EventType.KeyPressed:

                    if (Inputs.IsGameInput(InputType.Back, evt))
                    {
                        Window.Close();
                        NextScreen = ScreenType.GameScreen;
                        return true;
                    }

                    break;
            }
            
            return false;
        }

        const float VIEW_MOVE_MINOR_LIMIT = .08F;
        const float VIEW_MOVE_TRIGGER_LIMIT = 20F;
        void UpdateView(Time dt)
        {
            float moveX = 0.0f;
            float moveY = 0.0f;

            Vector2 p = Map.Combat.ViewFollowingTarget.Position;
            float velocity = 200F - CombatCursor.TRANSITION_VELOCITY;

            if (Math.Abs(p.X - GameView.Center.X) > VIEW_MOVE_TRIGGER_LIMIT)
                moveX = velocity * (p.X - GameView.Center.X) / 50f * GameDatas.WINDOW_WIDTH / GameDatas.WINDOW_HEIGHT * (float)dt.Value;
            if (Math.Abs(p.Y - GameView.Center.Y) > VIEW_MOVE_TRIGGER_LIMIT)
                moveY = velocity * (p.Y - GameView.Center.Y) / 50f * GameDatas.WINDOW_HEIGHT / GameDatas.WINDOW_WIDTH * (float)dt.Value;

            if (GameView.Center.X - GameView.Size.X / 2 + moveX < 0F)
            {
                GameView.Center = new Vector2(0F, GameView.Center.Y) + new Vector2(GameView.Size.X / 2F, 0F);
                moveX = 0.0f;
            }

            if (GameView.Center.X - GameView.Size.X / 2 + GameView.Size.X + moveX >= Map.Dimension.X)
            {
                GameView.Center = new Vector2(Map.Dimension.X, GameView.Center.Y) - new Vector2(GameView.Size.X / 2F, 0F);
                moveX = 0.0f;
            }

            if (GameView.Center.Y - GameView.Size.Y / 2 + moveY < 0F)
            {
                GameView.Center = new Vector2(GameView.Center.X, 0F) + new Vector2(0F, GameView.Size.Y / 2F);
                moveY = 0.0f;
            }

            if (GameView.Center.Y - GameView.Size.Y / 2 + GameView.Size.Y + moveY >= Map.Dimension.Y)
            {
                GameView.Center = new Vector2(GameView.Center.X, Map.Dimension.Y) - new Vector2(0F, GameView.Size.Y / 2F);
                moveY = 0.0f;
            }

            Gui.MoveGameView(new Vector2(moveX, moveY));
        }
    }
}
