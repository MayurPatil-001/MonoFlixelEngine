using Engine.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engine.Systems.FrontEnds
{
    public class PluginFrontEnd : CoreGameComponent
    {
        public List<FlxBasic> List { get; private set; } = new List<FlxBasic>();

        public PluginFrontEnd()
        {
            FlxTimer.GlobalManager = new FlxTimerManager();
            Add(FlxTimer.GlobalManager);

            //TODO:  Tween Manager
        }


        public override void Update(GameTime gameTime)
        {
            foreach (FlxBasic plugin in List)
                if (plugin.Exists && plugin.Active)
                    plugin.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (FlxBasic plugin in List)
                if (plugin.Exists && plugin.Visible)
                    plugin.Draw(gameTime);
            base.Draw(gameTime);
        }



        public T Add<T>(T newPlugin) where T : FlxBasic
        {
            foreach (var plugin in List)
                if (plugin.GetType().FullName.Equals(newPlugin.GetType().FullName))
                    return newPlugin;
            List.Add(newPlugin);
            return newPlugin;
        }

        public FlxBasic Get(Type classType)
        {
            foreach (var plugin in List)
                if (plugin.GetType() == classType)
                    return plugin;
            return null;
        }
        public FlxBasic Remove(FlxBasic plugin)
        {
            List.Remove(plugin);

            return plugin;
        }

        public bool RemoveType(Type classType)
        {
            // Don't add repeats
            bool results = false;
            int i = List.Count - 1;

            while (i >= 0)
            {
                if (List[i].GetType().Equals(classType))
                {
                    List.RemoveAt(i);
                    results = true;
                }
                i--;
            }
            return results;
        }


    }
}