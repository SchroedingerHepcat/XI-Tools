using FFACETools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroLimits.XITools
{
    public class XITools
    {
        /// <summary>
        /// Singleton instance of the current farming tools.
        /// </summary>
        private static XITools _farmingTools;

        /// <summary>
        /// The current fface instance bound to farming tools. 
        /// </summary>
        private static FFACE _fface;

        private XITools(FFACE fface)
        {
            _fface = fface;
            this.AbilityExecutor = new AbilityExecutor(fface);
            this.AbilityService = new AbilityService();
            this.CombatService = new CombatService(fface);
            this.RestingService = new RestingService(fface);
            this.UnitService = new UnitService(fface);
            this.ActionBlocked = new ActionBlocked(fface);
        }

        /// <summary>
        /// A single point of access method that returns a FarmingTools object. 
        /// Make sure you have set an FFACE object before calling this method. 
        /// </summary>
        /// <returns></returns>
        public static XITools GetInstance()
        {
            return _farmingTools;
        }

        /// <summary>
        /// A single point of access method that returns a FarmingTools object.
        /// The object returned will be based on the FFACE instance provided or
        /// if no object was previously created, it will create one for you. 
        /// </summary>
        /// <param name="fface"></param>
        /// <returns></returns>
        public static XITools GetInstance(FFACE fface)
        {
            if (_farmingTools == null || !_fface.Equals(fface))
            {
                _farmingTools = new XITools(fface);
            }

            return _farmingTools;
        }

        /// <summary>
        /// Provides access to FFACE memory reading api which returns details
        /// about various game environment objects. 
        /// </summary>
        public FFACE FFACE
        {
            get { return _fface; }
            set { _fface = value; }
        }

        /// <summary>
        /// Provides services for acquiring ability/spell data.
        /// </summary>
        public AbilityService AbilityService { get; set; }

        /// <summary>
        /// Provides the ability to executor abilities/spells.
        /// </summary>
        public AbilityExecutor AbilityExecutor { get; set; }

        /// <summary>
        /// Provides methods for performing battle.
        /// </summary>
        public CombatService CombatService { get; set; }

        /// <summary>
        /// Provides methods for resting our character.
        /// </summary>
        public RestingService RestingService { get; set; }

        /// <summary>
        /// Provide details about the units around us. 
        /// </summary>
        public UnitService UnitService { get; set; }

        /// <summary>
        /// Provides methods on whether an ability/spell is usable.
        /// </summary>
        public ActionBlocked ActionBlocked { get; set; }
    }
}
