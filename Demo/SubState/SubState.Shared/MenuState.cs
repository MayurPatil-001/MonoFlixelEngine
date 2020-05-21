using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;

namespace SubState.Shared
{
	public class MenuState : FlxState
	{

		// Link to the persistent substate (which will exist after closing)
		SubState persistentSubState;

		FlxButton openpersistentBtn;
		FlxButton openTempBtn;
		MySpriteGroup sprites;

		Color subStateColor;
		protected override void Create()
		{
			FlxG.Cameras.BgColor = Color.White;

			DestroySubStates = false;

			// Some test group of sprites, used for showing substate system features
			sprites = new MySpriteGroup(50);
			Add(sprites);

			subStateColor = new Color(128, 128, 128, 153);

			// We can create persistent substate and use it as many times as we want
			persistentSubState = new SubState(subStateColor);
			persistentSubState.isPersistent = true;

			openpersistentBtn = new FlxButton(20, 20, "OpenPersistent", OnpersistentClick);
			Add(openpersistentBtn);

			openTempBtn = new FlxButton(20, 60, "OpenTemp", OnTempClick);
			Add(openTempBtn);
			base.Create();
		}


		void OnTempClick()
		{
			FlxG.Log.Info("OnTempClick +" + GetHashCode());
			// This is temp substate, it will be destroyed after closing
			SubState tempState = new SubState(subStateColor);
			tempState.isPersistent = false;
			OpenSubState(tempState);
		}

		void OnpersistentClick()
		{
			FlxG.Log.Info("OnpersistentClick +" + GetHashCode());
			OpenSubState(persistentSubState);
		}


		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			sprites = null;

			// don't forget to destroy persistent substate
			persistentSubState.Dispose();
			persistentSubState = null;
		}
	}
}
