
/*///////////////////////////////////////////////////////////////////
<EasyFarm, general farming utility for FFXI.>
Copyright (C) <2013 - 2014>  <Zerolimits>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
*/
///////////////////////////////////////////////////////////////////

using FFACETools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroLimits.XITools
{
    /// <summary>
    /// A class for using abilties.
    /// </summary>

    public class AbilityExecutor
    {
        private FFACE _fface;

        private float PriorCastCountDown;

        public AbilityExecutor(FFACE fface)
        {
            this._fface = fface;
        }

        /// <summary>
        /// Performs a list of actions. 
        /// Could be spells or job abilities. 
        /// Does not validate actions.
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="unit"></param>
        public void ExecuteActions(List<Ability> actions)
        {
            foreach (var act in actions)
            {
                UseAbility(act);
            }
        }

        /// <summary>
        /// Attempts to use the passed in ability
        /// </summary>
        /// <param name="ability"></param>
        public bool UseAbility(Ability ability)
        {
            bool success = false;

            // Stop the bot from running so that we can cast. 
            _fface.Navigator.Reset();

            // Send it to the game
            _fface.Windower.SendString(ability.ToString());

            if (ability.IsSpell)
            {
                while (_fface.Player.CastCountDown != 0 ||
                    _fface.Player.CastCountDown != PriorCastCountDown)
                {
                    PriorCastCountDown = _fface.Player.CastCountDown;
                    System.Threading.Thread.Sleep(50);
                }
            }

            if (_fface.Player.CastCountDown == 0 && _fface.Player.CastCountDown != PriorCastCountDown) { success = true; }

            return success;
        }

        /// <summary>
        /// Checks to  see if we can cast/use 
        /// a job ability or spell.
        /// </summary>
        /// <param name="ability"></param>
        /// <returns></returns>
        public bool IsRecastable(Ability ability)
        {
            int recast = -1;

            // If a spell get spell recast
            if (ability.IsSpell) recast = _fface.Timer.GetSpellRecast((SpellList)ability.Index);

            // if ability get ability recast. 
            if (ability.IsAbility) recast = _fface.Timer.GetAbilityRecast((AbilityList)ability.Index);

            return recast == 0;
        }

        /// <summary>
        /// Returns the list of usable abilities
        /// </summary>
        /// <param name="Actions"></param>
        /// <returns></returns>
        public List<Ability> FilterValidActions(IList<Ability> Actions)
        {
            return Actions.Where(x => IsActionValid(x)).ToList();
        }

        /// <summary>
        /// Determines whether a spell or ability can be used based on...
        /// 1) It retrieved a non-null ability/spell from the resource files.
        /// 2) The ability is recastable.
        /// 3) The user has the mp or tp for the move.
        /// 4) We don't have a debuff like amnesia that stops us from using it. 
        /// </summary>
        /// <param name="action"></param>
        /// <returns>True for usable, False for unusable</returns>
        public bool IsActionValid(Ability action)
        {
            // Ability valid check
            if (!action.IsValidName) return false;

            // Recast Check
            if (!IsRecastable(action)) return false;

            // MP Check
            if (action.MPCost > _fface.Player.MPCurrent) return false;

            // TP Check
            if (action.TPCost > _fface.Player.TPCurrent) return false;

            // Determine whether we have an debuff that blocks us from casting spells. 
            if (action.IsSpell)
            {
                if (XITools.GetInstance(_fface).ActionBlocked.AreSpellsBlocked) return false;
            }

            // Determines if we have a debuff that blocks us from casting abilities. 
            if (action.IsAbility)
            {
                if (XITools.GetInstance(_fface).ActionBlocked.AreAbilitiesBlocked) return false;
            }

            return true;
        }
    }
}
