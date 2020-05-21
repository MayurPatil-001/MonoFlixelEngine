using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;

namespace SubState.Shared
{
	class SubState : FlxSubState
	{
		// Some test sprite, showing that if the state is persistent (not destroyed after closing)
		// then it will save it's position (and all other properties)
		FlxSprite testSprite;

		FlxButton closeBtn;
		FlxButton switchParentDrawingBtn;
		FlxButton switchParentUpdatingBtn;

		// just a helper flag, showing if this substate is persistent or not
		public bool isPersistent = false;

		public SubState(Color bgColor):base(bgColor)
		{

		}

		protected override void Create()
		{
			base.Create();

			closeBtn = new FlxButton(FlxG.Width * 0.5f - 40f, FlxG.Height * 0.5f, "Close", OnClick);
			Add(closeBtn);

			//switchParentDrawingBtn = new FlxButton(closeBtn.X, closeBtn.Y + 40, "SwitchDraw", OnSwitchDraw);
			//Add(switchParentDrawingBtn);

			//switchParentUpdatingBtn = new FlxButton(switchParentDrawingBtn.X, switchParentDrawingBtn.Y + 40, "SwitchUpdate", OnSwitchUpdate);
			//Add(switchParentUpdatingBtn);

			//testSprite = new FlxSprite(0, 10);
			//testSprite.Velocity.X = 20;
			//Add(testSprite);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			//if (testSprite.X > FlxG.Width)
			//{
			//	testSprite.X = -testSprite.Width;
			//}
		}

		void OnSwitchUpdate()
		{
			FlxG.Log.Info("OnSwitchUpdate");
			if (ParentState != null)
			{
				// you can keep updating parent state if you want to, but keep in mind that
				// if you will update parent state then you will update buttons in it,
				// so you need to deactivate buttons in parent state
				ParentState.PersistentUpdate = !ParentState.PersistentUpdate;
			}
		}

		void OnSwitchDraw()
		{
			FlxG.Log.Info("OnSwitchDraw");
			if (ParentState != null)
			{
				// you can keep drawing parent state if you want to 
				// (for example, when substate have transparent background color)
				ParentState.PersistentDraw = !ParentState.PersistentDraw;
			}
		}


		void OnClick()
		{
			// if you will pass 'true' (which is by default) into close() method then this substate will be destroyed
			// but when you'll pass 'false' then you should destroy it manually
			Close();
		}

		// This function will be called by substate right after substate will be closed
		public static void OnSubstateClose()
		{
			//FlxG.fade(FlxG.BLACK, 1, true);
		}
	}
}
