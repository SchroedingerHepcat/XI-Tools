
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
        private const short MOBARRAY_MAX = 768;
        private static FFACE _fface;

        #endregion

        public UnitService(FFACE session)
        {
            _fface = session;
            Unit.Session = _fface;
            _unitArray = new Unit[MOBARRAY_MAX];

            // Create units
            for (int id = 0; id < MOBARRAY_MAX; id++)
            {
                _unitArray[id] = Unit.CreateUnit(id);
            }

            this.DetectionDistance = 17;
            this.HeightThreshold = 5;
            this.TargetMobs = new List<string>();
            this.IgnoredMobs = new List<string>();
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
        /// Does there exist a mob that has aggroed in general.
        /// </summary>
        public bool HasAggro
        {
            get
            {
                foreach (var Monster in ValidMobs)
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
                foreach (var Monster in ValidMobs)
                {
                    if (Monster.IsClaimed)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public Unit[] ValidMobs
        {
            get
            {
                return _unitArray
                    .Where(x => IsValid(x))
                    .ToArray();
            }
        }

        public Unit[] UnitArray
        { 
            get 
            { 
                return _unitArray; 
            } 
        }

        #endregion

        public bool IsValid(Unit unit)
        {
            // Gain performance by only checking mobs that are active
            if (!unit.IsActive) return false; 

            // Mob not in reach height wise. 
            if (HeightThreshold > unit.YDifference) return false;

            // mob is not blank.
            if (unit.NPCBit == 0) return false;

            // If the mob is dead.
            if(unit.IsDead) return false;

            // Mob not out target.
            if (!TargetMobs.Contains(unit.Name)) return false;

            // If the mob is ignored and there are no targets
            if(IgnoredMobs.Contains(unit.Name)) return false;

            return true;
        }

        public Unit GetTarget(Func<Unit, bool> filter)
        {
            return UnitArray.Where(filter).OrderBy(x => x.Distance)
                .FirstOrDefault();
        }
    }
}
