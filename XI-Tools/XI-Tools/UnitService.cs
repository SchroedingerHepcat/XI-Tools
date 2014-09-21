
/*///////////////////////////////////////////////////////////////////
<EasyFarm, general farming utility for FFXI.>
Copyright (C) <2013>  <Zerolimits>

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

﻿using System;
using System.Linq;
using FFACETools;
using System.Collections.Generic;

namespace ZeroLimits.XITools
{
    public class UnitService
    {
        #region Members
        private static Unit[] _unitArray;
        private const short UNIT_ARRAY_MAX = Constants.UNIT_ARRAY_MAX;
        private static FFACE _fface;

        #endregion

        public UnitService(FFACE session)
        {
            _fface = session;
            Unit.Session = _fface;
            _unitArray = new Unit[UNIT_ARRAY_MAX];

            // Create units
            for (int id = 0; id < UNIT_ARRAY_MAX; id++)
            {
                _unitArray[id] = Unit.CreateUnit(id);
            }

            this.DetectionDistance = 17;
            this.HeightThreshold = 5;
            this.TargetMobs = new List<string>();
            this.IgnoredMobs = new List<string>();
            this.UnitFilter = DefaultFilter(_fface);
        }

        #region Properties

        /// <summary>
        /// Max height difference a mob can be from the player. 
        /// </summary>
        public double HeightThreshold { get; set; }

        /// <summary>
        /// Max distance a mob will be considered valid.
        /// </summary>
        public double DetectionDistance { get; set; }

        /// <summary>
        /// List of mobs we'd like to kill. 
        /// </summary>
        public List<String> TargetMobs { get; set; }

        /// <summary>
        ///  List of mobs we'd like to ignore. 
        /// </summary>
        public List<String> IgnoredMobs { get; set; }

        /// <summary>
        /// Used to filter units based on what the user thinks is valid. 
        /// Sets an optional filter to be used. 
        /// </summary>
        public Func<Unit, bool> UnitFilter { get; set; }

        /// <summary>
        /// Does there exist a mob that has aggroed in general.
        /// </summary>
        public bool HasAggro
        {
            get
            {
                foreach (var Monster in FilteredArray)
                {
                    if (Monster.HasAggroed)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Do we have claim on any mob?
        /// </summary>
        public bool HasClaim
        {
            get
            {
                foreach (var Monster in FilteredArray)
                {
                    if (Monster.IsClaimed)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Returns the list of filter units.
        /// </summary>
        public Unit[] FilteredArray
        {
            get
            {
                return _unitArray
                    .Where(x => IsValid(x))
                    .ToArray();
            }
        }

        /// <summary>
        /// Retrieves the list of UNITs
        /// </summary>
        public Unit[] UnitArray
        {
            get
            {
                return _unitArray;
            }
        }

        /// <summary>
        /// Retrieves the list of MOBs.
        /// </summary>
        public Unit[] MOBArray
        {
            get 
            {
                return UnitArray
                    .Where(x => x.NPCType.Equals(NPCType.Mob))
                    .ToArray();
            }
        }

        /// <summary>
        /// Retrieves the lsit of PCs.
        /// </summary>
        public Unit[] PCArray
        {
            get 
            {
                return UnitArray
                    .Where(x => x.NPCType.Equals(NPCType.PC))
                    .ToArray();
            }
        }

        #endregion

        public bool IsValid(Unit unit)
        {
            return UnitFilter(unit);
        }

        /// <summary>
        /// Returns matched units that meet the filters requirements. 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Unit[] GetUnits(Func<Unit, bool> filter)
        {
            return UnitArray.Where(filter).ToArray();
        }

        /// <summary>
        /// Returns matched units that meet the filters requirements 
        /// and then orders by the passed function.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public Unit[] GetUnits(Func<Unit, bool> filter, Func<Unit, object> orderby)
        {
            return UnitArray.Where(filter).OrderBy(orderby).ToArray();
        }

        public Unit GetTarget(Func<Unit, bool> filter)
        {
            return GetUnits(filter).FirstOrDefault();
        }

        /// <summary>
        /// Matches mobs against the "filter" and then orders them "orderby" 
        /// and returns the first match.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public Unit GetTarget(Func<Unit, bool> filter, Func<Unit, object> orderby)
        {
            return GetUnits(filter).OrderBy(orderby).FirstOrDefault();
        }

        // The default mob filter used in filtering the mobs. 
        public Func<Unit, bool> DefaultFilter(FFACE fface)
        {
            return new Func<Unit, bool>((Unit unit) =>
            {
                // If the mob is false... 
                if (unit == null) return false;

                // Gain performance by only checking mobs that are active
                if (!unit.IsActive) return false;

                // Mob not in reach height wise. 
                if (unit.YDifference > HeightThreshold) return false;

                // Allow for mobs with an npc bit of sometimes 4 (colibri) 
                // and ignore mobs that are invisible npcbit = 0
                if (unit.NPCBit <= 0 || unit.NPCBit >= 16) return false;

                // If the mob is dead.
                if (unit.IsDead) return false;

                // Mob not out target.
                if (!TargetMobs.Contains(unit.Name) && TargetMobs.Count > 0) return false;

                // If the mob is ignored and there are no targets
                if (IgnoredMobs.Contains(unit.Name)) return false;

                return true;
            });
        }
    }
}
