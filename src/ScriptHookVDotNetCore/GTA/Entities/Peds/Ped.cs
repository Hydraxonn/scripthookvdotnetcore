//
// Copyright (C) 2015 crosire & contributors
// License: https://github.com/crosire/scripthookvdotnet#license
//

using GTA.Math;
using GTA.NaturalMotion;

namespace GTA
{
    public sealed class Ped : Entity
    {
        #region Fields

        TaskInvoker _tasks;
        Euphoria _euphoria;
        WeaponCollection _weapons;
        Style _style;
        PedBoneCollection _pedBones;

        internal static readonly string[] _speechModifierNames =
        {
            "SPEECH_PARAMS_STANDARD",
            "SPEECH_PARAMS_ALLOW_REPEAT",
            "SPEECH_PARAMS_BEAT",
            "SPEECH_PARAMS_FORCE",
            "SPEECH_PARAMS_FORCE_FRONTEND",
            "SPEECH_PARAMS_FORCE_NO_REPEAT_FRONTEND",
            "SPEECH_PARAMS_FORCE_NORMAL",
            "SPEECH_PARAMS_FORCE_NORMAL_CLEAR",
            "SPEECH_PARAMS_FORCE_NORMAL_CRITICAL",
            "SPEECH_PARAMS_FORCE_SHOUTED",
            "SPEECH_PARAMS_FORCE_SHOUTED_CLEAR",
            "SPEECH_PARAMS_FORCE_SHOUTED_CRITICAL",
            "SPEECH_PARAMS_FORCE_PRELOAD_ONLY",
            "SPEECH_PARAMS_MEGAPHONE",
            "SPEECH_PARAMS_HELI",
            "SPEECH_PARAMS_FORCE_MEGAPHONE",
            "SPEECH_PARAMS_FORCE_HELI",
            "SPEECH_PARAMS_INTERRUPT",
            "SPEECH_PARAMS_INTERRUPT_SHOUTED",
            "SPEECH_PARAMS_INTERRUPT_SHOUTED_CLEAR",
            "SPEECH_PARAMS_INTERRUPT_SHOUTED_CRITICAL",
            "SPEECH_PARAMS_INTERRUPT_NO_FORCE",
            "SPEECH_PARAMS_INTERRUPT_FRONTEND",
            "SPEECH_PARAMS_INTERRUPT_NO_FORCE_FRONTEND",
            "SPEECH_PARAMS_ADD_BLIP",
            "SPEECH_PARAMS_ADD_BLIP_ALLOW_REPEAT",
            "SPEECH_PARAMS_ADD_BLIP_FORCE",
            "SPEECH_PARAMS_ADD_BLIP_SHOUTED",
            "SPEECH_PARAMS_ADD_BLIP_SHOUTED_FORCE",
            "SPEECH_PARAMS_ADD_BLIP_INTERRUPT",
            "SPEECH_PARAMS_ADD_BLIP_INTERRUPT_FORCE",
            "SPEECH_PARAMS_FORCE_PRELOAD_ONLY_SHOUTED",
            "SPEECH_PARAMS_FORCE_PRELOAD_ONLY_SHOUTED_CLEAR",
            "SPEECH_PARAMS_FORCE_PRELOAD_ONLY_SHOUTED_CRITICAL",
            "SPEECH_PARAMS_SHOUTED",
            "SPEECH_PARAMS_SHOUTED_CLEAR",
            "SPEECH_PARAMS_SHOUTED_CRITICAL",
        };

        #endregion

        internal Ped(int handle) : base(handle)
        {
        }

        /// <summary>
        /// Spawn an identical clone of this <see cref="Ped"/>.
        /// </summary>
        /// <param name="heading">The direction the clone should be facing.</param>
        public Ped Clone(float heading = 0.0f)
        {
            return new Ped(Call<int>(Hash.CLONE_PED, Handle, heading, false, false));
        }

        /// <summary>
        /// Kills this <see cref="Ped"/> immediately.
        /// </summary>
        public void Kill()
        {
            Health = 0;
        }

        /// <summary>
        /// Resurrects this <see cref="Ped"/> from death.
        /// </summary>
        public void Resurrect()
        {
            int health = MaxHealth;
            bool isCollisionEnabled = IsCollisionEnabled;

            Call(Hash.RESURRECT_PED, Handle);
            Health = MaxHealth = health;
            IsCollisionEnabled = isCollisionEnabled;
            Call(Hash.CLEAR_PED_TASKS_IMMEDIATELY, Handle);
        }

        /// <summary>
        /// Determines if this <see cref="Ped"/> exists.
        /// You should ensure <see cref="Ped"/>s still exist before manipulating them or getting some values for them on every tick, since some native functions may crash the game if invalid entity handles are passed.
        /// </summary>
        /// <returns><see langword="true" /> if this <see cref="Ped"/> exists; otherwise, <see langword="false" /></returns>
        /// <seealso cref="Entity.IsDead"/>
        /// <seealso cref="IsInjured"/>
        public new bool Exists()
        {
            return EntityType == EntityType.Ped;
        }

        private IntPtr PedIntelligenceAddress
        {
            get
            {
                var address = MemoryAddress;
                if (address == IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }

                return NativeMemory.ReadAddress(address + NativeMemory.PedIntelligenceOffset);
            }
        }

        #region Styling

        /// <summary>
        /// Gets a value indicating whether this <see cref="Ped"/> is human.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if this <see cref="Ped"/> is human; otherwise, <see langword="false" />.
        /// </value>
        public bool IsHuman => Call<bool>(Hash.IS_PED_HUMAN, Handle);

        public bool IsCuffed => Call<bool>(Hash.IS_PED_CUFFED, Handle);

        public bool CanWearHelmet
        {
            set => Call(Hash.SET_PED_HELMET, Handle, value);
        }

        public bool IsWearingHelmet => Call<bool>(Hash.IS_PED_WEARING_HELMET, Handle);

        public void ClearBloodDamage()
        {
            Call(Hash.CLEAR_PED_BLOOD_DAMAGE, Handle);
        }

        public void ClearVisibleDamage()
        {
            Call(Hash.RESET_PED_VISIBLE_DAMAGE, Handle);
        }

        public void GiveHelmet(bool canBeRemovedByPed, Helmet helmetType, int textureIndex)
        {
            Call(Hash.GIVE_PED_HELMET, Handle, !canBeRemovedByPed, helmetType, textureIndex);
        }

        public void RemoveHelmet(bool instantly)
        {
            Call(Hash.REMOVE_PED_HELMET, Handle, instantly);
        }

        /// <summary>
        /// Opens a list of clothing and prop configurations that this <see cref="Ped"/> can wear.
        /// </summary>
        public Style Style => _style ?? (_style = new Style(this));

        /// <summary>
        /// Gets the gender of this <see cref="Ped"/>.
        /// </summary>
        public Gender Gender => Call<bool>(Hash.IS_PED_MALE, Handle) ? Gender.Male : Gender.Female;

        /// <summary>
        /// Gets or sets the how much sweat should be rendered on this <see cref="Ped"/>.
        /// </summary>
        /// <value>
        /// The sweat from 0 to 100, 0 being no sweat, 100 being saturated.
        /// </value>
        public float Sweat
        {
            get
            {
                var address = MemoryAddress;
                if (address == IntPtr.Zero || NativeMemory.SweatOffset == 0)
                {
                    return 0.0f;
                }

                return NativeMemory.ReadFloat(address + NativeMemory.SweatOffset);
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                if (value > 100)
                {
                    value = 100;
                }

                Call(Hash.SET_PED_SWEAT, Handle, value);
            }
        }

        /// <summary>
        /// Sets how high up on this <see cref="Ped"/>s body water should be visible.
        /// </summary>
        /// <value>
        /// The height ranges from 0.0f to 1.99f, 0.0f being no water visible, 1.99f being covered in water.
        /// </value>
        public float WetnessHeight
        {
            set
            {
                if (value == 0.0f)
                {
                    Call(Hash.CLEAR_PED_WETNESS, Handle);
                }
                else
                {
                    Call<float>(Hash.SET_PED_WETNESS_HEIGHT, Handle, value);
                }
            }
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Gets or sets how much armor this <see cref="Ped"/> is wearing as an <see cref="int"/>.
        /// </summary>
        /// <remarks>if you need to get or set the value precisely, use <see cref="ArmorFloat"/> instead.</remarks>
        /// <value>
        /// The armor as an <see cref="int"/>.
        /// </value>
        public int Armor
        {
            get => Call<int>(Hash.GET_PED_ARMOUR, Handle);
            set => Call(Hash.SET_PED_ARMOUR, Handle, value);
        }

        /// <summary>
        /// Gets or sets how much Armor this <see cref="Ped"/> is wearing as a <see cref="float"/>.
        /// </summary>
        /// <value>
        /// The armor as a <see cref="float"/>.
        /// </value>
        public float ArmorFloat
        {
            get
            {
                var address = MemoryAddress;
                if (address == IntPtr.Zero || NativeMemory.ArmorOffset == 0)
                {
                    return 0.0f;
                }

                return NativeMemory.ReadFloat(address + NativeMemory.ArmorOffset);
            }
            set
            {
                var address = MemoryAddress;
                if (address == IntPtr.Zero || NativeMemory.ArmorOffset == 0)
                {
                    return;
                }

                NativeMemory.WriteFloat(address + NativeMemory.ArmorOffset, value);
            }
        }

        /// <summary>
        /// Gets or sets how much money this <see cref="Ped"/> is carrying.
        /// </summary>
        public int Money
        {
            get => Call<int>(Hash.GET_PED_MONEY, Handle);
            set => Call(Hash.SET_PED_MONEY, Handle, value);
        }

        /// <summary>
        /// Gets or sets the maximum health of this <see cref="Ped"/> as an <see cref="int"/>.
        /// </summary>
        /// <value>
        /// The maximum health as an <see cref="int"/>.
        /// </value>
        public override int MaxHealth
        {
            get => Call<int>(Hash.GET_PED_MAX_HEALTH, Handle);
            set => Call(Hash.SET_PED_MAX_HEALTH, Handle, value);
        }

        public bool IsPlayer => Call<bool>(Hash.IS_PED_A_PLAYER, Handle);

        public bool GetConfigFlag(int flagID)
        {
            return Call<bool>(Hash.GET_PED_CONFIG_FLAG, Handle, flagID, true);
        }

        public void SetConfigFlag(int flagID, bool value)
        {
            Call(Hash.SET_PED_CONFIG_FLAG, Handle, flagID, value);
        }

        public void ResetConfigFlag(int flagID)
        {
            Call(Hash.SET_PED_RESET_FLAG, Handle, flagID, true);
        }

        /// <summary>
        /// Sets a value indicating whether this <see cref="Entity"/> is persistent.
        /// Unlike <see cref="Entity.IsPersistent"/>, calling this method does not affect assigned tasks.
        /// </summary>
        public void SetIsPersistentNoClearTask(bool value)
        {
            if (value)
            {
                PopulationType = EntityPopulationType.Mission;
            }
            else
            {
                PopulationType = EntityPopulationType.RandomAmbient;
            }
        }

        /// <summary>
        /// Gets a collection of the <see cref="PedBone"/>s in this <see cref="Ped"/>.
        /// </summary>
        public new PedBoneCollection Bones => _pedBones ?? (_pedBones = new PedBoneCollection(this));

        #endregion

        #region Tasks

        public bool IsIdle => !IsInjured && !IsRagdoll && !IsInAir && !IsOnFire && !IsDucking &&
                              !IsGettingIntoVehicle && !IsInCombat && !IsInMeleeCombat &&
                              (!IsInVehicle() || IsSittingInVehicle());

        public bool IsProne => Call<bool>(Hash.IS_PED_PRONE, Handle);

        public bool IsGettingUp => Call<bool>(Hash.IS_PED_GETTING_UP, Handle);

        public bool IsDiving => Call<bool>(Hash.IS_PED_DIVING, Handle);

        public bool IsJumping => Call<bool>(Hash.IS_PED_JUMPING, Handle);

        public bool IsFalling => Call<bool>(Hash.IS_PED_FALLING, Handle);

        public bool IsVaulting => Call<bool>(Hash.IS_PED_VAULTING, Handle);

        public bool IsClimbing => Call<bool>(Hash.IS_PED_CLIMBING, Handle);

        public bool IsWalking => Call<bool>(Hash.IS_PED_WALKING, Handle);

        public bool IsRunning => Call<bool>(Hash.IS_PED_RUNNING, Handle);

        public bool IsSprinting => Call<bool>(Hash.IS_PED_SPRINTING, Handle);

        public bool IsStopped => Call<bool>(Hash.IS_PED_STOPPED, Handle);

        public bool IsSwimming => Call<bool>(Hash.IS_PED_SWIMMING, Handle);

        public bool IsSwimmingUnderWater => Call<bool>(Hash.IS_PED_SWIMMING_UNDER_WATER, Handle);

        public bool IsDucking
        {
            get => Call<bool>(Hash.IS_PED_DUCKING, Handle);
            set => Call(Hash.SET_PED_DUCKING, Handle, value);
        }

        public bool IsHeadtracking(Entity entity)
        {
            return Call<bool>(Hash.IS_PED_HEADTRACKING_ENTITY, Handle, entity.Handle);
        }

        /// <summary>
        /// <para>
        /// Sets whether this <see cref="Ped"/> keeps their tasks when they are marked as no longer needed by <see cref="Entity.MarkAsNoLongerNeeded"/> or gets cleaned up by the mission script.
        /// Despite the property name, this property does not determine whether permanent events can interrupt the <see cref="Ped"/>'s tasks (e.g. seeing hated peds or getting shot at).
        /// </para>
        /// <para>
        /// If set to <see langword="false" />, this <see cref="Ped"/>'s task will be immediately cleared and start some ambient tasks (most likely start wandering) when they are marked as no longer needed.
        /// </para>
        /// <para>
        /// If set to <see langword="true" />, this <see cref="Ped"/> will keep their task until they have nothing to do (where their task stacks only contains <c>CTaskDoNothing</c>).
        /// Once this <see cref="Ped"/> has nothing to do, their task will clear and they'll start some ambient tasks (one-time-only).
        /// </para>
        /// </summary>

        public bool AlwaysKeepTask
        {
            set => Call(Hash.SET_PED_KEEP_TASK, Handle, value);
        }

        /// <summary>
        /// Sets whether permanent events are blocked for this <see cref="Ped"/>.
        /// <para>
        /// If set to <see langword="true" />, this <see cref="Ped"/> will no longer react to permanent events and will only do as they're told.
        /// For example, the <see cref="Ped"/> will not flee when get shot at and they will not begin combat even if the decision maker specifies that seeing a hated ped should.
        /// However, the <see cref="Ped"/> will still respond to temporary events like walking around other peds or vehicles even if this property is set to <see langword="true" />.
        /// </para>
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if permanent events are blocked; otherwise, <see langword="false" />.
        /// </value>
        public bool BlockPermanentEvents
        {
            set => Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, Handle, value);
        }

        /// <summary>
        /// Opens a list of <see cref="TaskInvoker"/> that this <see cref="Ped"/> can carry out.
        /// </summary>
        public TaskInvoker Task => _tasks ?? (_tasks = new TaskInvoker(this));

        /// <summary>
        /// Gets the stage of the <see cref="TaskSequence"/> this <see cref="Ped"/> is currently executing.
        /// </summary>
        public int TaskSequenceProgress => Call<int>(Hash.GET_SEQUENCE_PROGRESS, Handle);

        #endregion

        #region Euphoria & Ragdoll

        /// <summary>
        /// Enables this <see cref="Ped"/>'s ragdoll by starting a ragdoll task and applying to this <see cref="Ped"/>.
        /// If <paramref name="ragdollType"/> is not set to <see cref="RagdollType.Relax"/> or <see cref="RagdollType.ScriptControl"/>, the ragdoll behavior for <see cref="RagdollType.Balance"/> will be used.
        /// </summary>
        public void Ragdoll(int duration = -1, RagdollType ragdollType = RagdollType.Relax)
        {
            CanRagdoll = true;
            Call(Hash.SET_PED_TO_RAGDOLL, Handle, duration, duration, ragdollType, false, false, false);
        }

        public void CancelRagdoll()
        {
            Call(Hash.SET_PED_TO_RAGDOLL, Handle, 1, 1, 1, false, false, false);
        }

        public bool IsRagdoll => Call<bool>(Hash.IS_PED_RAGDOLL, Handle);

        public bool CanRagdoll
        {
            get => Call<bool>(Hash.CAN_PED_RAGDOLL, Handle);
            set => Call(Hash.SET_PED_CAN_RAGDOLL, Handle, value);
        }

        /// <summary>
        /// Opens a list of <see cref="GTA.NaturalMotion.Euphoria"/> Helpers which can be applied to this <see cref="Ped"/>.
        /// </summary>
        public Euphoria Euphoria => _euphoria ?? (_euphoria = new Euphoria(this));

        #endregion

        #region Weapon Interaction

        /// <summary>
        /// Gets or sets how accurate this <see cref="Ped"/>s shooting ability is.
        ///  The higher the value of this property is, the more likely it is that this <see cref="Ped"/> will shoot at exactly where they are aiming at.
        /// </summary>
        /// <value>
        /// The accuracy from 0 to 100, 0 being very inaccurate, which means this <see cref="Ped"/> cannot shoot at exactly where they are aiming at,
        ///  100 being perfectly accurate.
        /// </value>
        public int Accuracy
        {
            get => Call<int>(Hash.GET_PED_ACCURACY, Handle);
            set => Call(Hash.SET_PED_ACCURACY, Handle, value);
        }

        /// <summary>
        /// Sets the rate this <see cref="Ped"/> will shoot at.
        /// </summary>
        /// <value>
        /// The shoot rate from 0.0f to 1000.0f, 100.0f is the default value.
        /// </value>
        public int ShootRate
        {
            set => Call(Hash.SET_PED_SHOOT_RATE, Handle, value);
        }

        /// <summary>
        /// Gets of sets the pattern this <see cref="Ped"/> uses to fire weapons.
        /// </summary>
        public FiringPattern FiringPattern
        {
            get
            {
                if (NativeMemory.FiringPatternOffset == 0)
                    return 0.0f;

                var address = PedIntelligenceAddress;
                if (address == IntPtr.Zero)
                    return 0.0f;

                return (FiringPattern)NativeMemory.ReadInt32(address + NativeMemory.FiringPatternOffset);
            }
            set => Call(Hash.SET_PED_FIRING_PATTERN, Handle, value);
        }

        /// <summary>
        /// Gets a collection of all this <see cref="Ped"/>s <see cref="Weapon"/>s.
        /// </summary>
        public WeaponCollection Weapons => _weapons ?? (_weapons = new WeaponCollection(this));

        /// <summary>
        /// Gets the vehicle weapon this <see cref="Ped"/> is using.
        /// <remarks>The vehicle weapon, returns <see cref="VehicleWeaponHash.Invalid"/> if this <see cref="Ped"/> isnt using a vehicle weapon.</remarks>
        /// </summary>
        public VehicleWeaponHash VehicleWeapon
        {
            get
            {
                unsafe
                {
                    int hash;
                    return Call<bool>(Hash.GET_CURRENT_PED_VEHICLE_WEAPON, Handle, &hash)
                        ? (VehicleWeaponHash)hash
                        : VehicleWeaponHash.Invalid;
                }
            }
            set { Call(Hash.SET_CURRENT_PED_VEHICLE_WEAPON, Handle, value); }
        }

        /// <summary>
        /// Sets if this <see cref="Ped"/> can switch between different weapons.
        /// </summary>
        public bool CanSwitchWeapons
        {
            set => Call(Hash.SET_PED_CAN_SWITCH_WEAPON, Handle, value);
        }

        #endregion

        #region Vehicle Interaction

        public bool IsOnBike => Call<bool>(Hash.IS_PED_ON_ANY_BIKE, Handle);

        public bool IsOnFoot => Call<bool>(Hash.IS_PED_ON_FOOT, Handle);

        public bool IsInSub => Call<bool>(Hash.IS_PED_IN_ANY_SUB, Handle);

        public bool IsInTaxi => Call<bool>(Hash.IS_PED_IN_ANY_TAXI, Handle);

        public bool IsInTrain => Call<bool>(Hash.IS_PED_IN_ANY_TRAIN, Handle);

        public bool IsInHeli => Call<bool>(Hash.IS_PED_IN_ANY_HELI, Handle);

        public bool IsInPlane => Call<bool>(Hash.IS_PED_IN_ANY_PLANE, Handle);

        public bool IsInFlyingVehicle => Call<bool>(Hash.IS_PED_IN_FLYING_VEHICLE, Handle);

        public bool IsInBoat => Call<bool>(Hash.IS_PED_IN_ANY_BOAT, Handle);

        public bool IsInPoliceVehicle => Call<bool>(Hash.IS_PED_IN_ANY_POLICE_VEHICLE, Handle);

        public bool IsGettingIntoVehicle => Call<bool>(Hash.IS_PED_GETTING_INTO_A_VEHICLE, Handle);

        /// <summary>
        /// Gets a value indicating whether this <see cref="Ped"/> is jumping out of their vehicle.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this <see cref="Ped"/> is jumping out of their vehicle; otherwise, <see langword="false" />.
        /// </value>
        public bool IsJumpingOutOfVehicle => Call<bool>(Hash.IS_PED_JUMPING_OUT_OF_VEHICLE, Handle);

        public bool IsTryingToEnterALockedVehicle => Call<bool>(Hash.IS_PED_TRYING_TO_ENTER_A_LOCKED_VEHICLE, Handle);

        public bool CanBeDraggedOutOfVehicle
        {
            set => Call(Hash.SET_PED_CAN_BE_DRAGGED_OUT, Handle, value);
        }

        public bool CanBeKnockedOffBike
        {
            set => Call(Hash.SET_PED_CAN_BE_KNOCKED_OFF_VEHICLE, Handle, !value);
        }

        public bool CanFlyThroughWindscreen
        {
            get => Call<bool>(Hash.GET_PED_CONFIG_FLAG, Handle, 32, true);
            set => Call(Hash.SET_PED_CONFIG_FLAG, Handle, 32, value);
        }

        public bool IsInVehicle()
        {
            return Call<bool>(Hash.IS_PED_IN_ANY_VEHICLE, Handle, 0);
        }

        public bool IsInVehicle(Vehicle vehicle)
        {
            return Call<bool>(Hash.IS_PED_IN_VEHICLE, Handle, vehicle.Handle, 0);
        }

        public bool IsSittingInVehicle()
        {
            return Call<bool>(Hash.IS_PED_SITTING_IN_ANY_VEHICLE, Handle);
        }

        public bool IsSittingInVehicle(Vehicle vehicle)
        {
            return Call<bool>(Hash.IS_PED_SITTING_IN_VEHICLE, Handle, vehicle.Handle);
        }

        public void SetIntoVehicle(Vehicle vehicle, VehicleSeat seat)
        {
            Call(Hash.SET_PED_INTO_VEHICLE, Handle, vehicle.Handle, seat);
        }

        /// <summary>
        /// Gets the last <see cref="Vehicle"/> this <see cref="Ped"/> used.
        /// </summary>
        /// <remarks>returns <see langword="null" /> if the last vehicle doesn't exist.</remarks>
        public Vehicle LastVehicle
        {
            get
            {
                var address = MemoryAddress;
                if (address == IntPtr.Zero)
                {
                    return null;
                }

                // GET_VEHICLE_PED_IS_IN isn't reliable at getting last vehicle since it returns 0 when the ped is going to a door of some vehicle or opening one.
                // Also, the native returns the vehicle's handle the ped is getting in when ped is getting in it (which is not the last vehicle), though the 2nd parameter name is supposed to be "ConsiderEnteringAsInVehicle" as a leaked header suggests.
                var vehicleHandle = NativeMemory.GetLastVehicleHandleOfPed(address);
                return vehicleHandle != 0 ? new Vehicle(vehicleHandle) : null;
            }
        }

        /// <summary>
        /// Gets the current <see cref="Vehicle"/> this <see cref="Ped"/> is using.
        /// </summary>
        /// <remarks>returns <see langword="null" /> if this <see cref="Ped"/> isn't in a <see cref="Vehicle"/>.</remarks>
        public Vehicle CurrentVehicle
        {
            get
            {
                // In b2699, GET_VEHICLE_PED_IS_IN always returns the last vehicle without checking the driving flag even when the 2nd argument is set to false.
                var address = MemoryAddress;
                if (address == IntPtr.Zero)
                {
                    return null;
                }

                var vehicleHandle = NativeMemory.GetVehicleHandlePedIsIn(address);
                return vehicleHandle != 0 ? new Vehicle(vehicleHandle) : null;
            }
        }

        /// <summary>
        /// Gets the <see cref="Vehicle"/> this <see cref="Ped"/> is trying to enter.
        /// </summary>
        /// <remarks>returns <see langword="null" /> if this <see cref="Ped"/> isn't trying to enter a <see cref="Vehicle"/>.</remarks>
        public Vehicle VehicleTryingToEnter
        {
            get
            {
                var veh = new Vehicle(Call<int>(Hash.GET_VEHICLE_PED_IS_TRYING_TO_ENTER, Handle));
                return veh.Exists() ? veh : null;
            }
        }

        /// <summary>
        /// Gets the <see cref="VehicleSeat"/> this <see cref="Ped"/> is in.
        /// </summary>
        /// <value>
        /// The <see cref="VehicleSeat"/> this <see cref="Ped"/> is in if this <see cref="Ped"/> is in a <see cref="Vehicle"/>; otherwise, <see cref="VehicleSeat.None"/>.
        /// </value>
        public VehicleSeat SeatIndex
        {
            get
            {
                var address = MemoryAddress;
                if (address == IntPtr.Zero || NativeMemory.SeatIndexOffset == 0)
                {
                    return VehicleSeat.None;
                }

                int seatIndex = (sbyte)NativeMemory.ReadByte(address + NativeMemory.SeatIndexOffset);
                return (seatIndex >= 0 && IsInVehicle()) ? (VehicleSeat)(seatIndex - 1) : VehicleSeat.None;
            }
        }

        #endregion

        #region Driving

        public float DrivingSpeed
        {
            set => Call(Hash.SET_DRIVE_TASK_CRUISE_SPEED, Handle, value);
        }

        /// <summary>
        /// Sets the maximum driving speed this <see cref="Ped"/> can drive at.
        /// </summary>
        public float MaxDrivingSpeed
        {
            set => Call(Hash.SET_DRIVE_TASK_MAX_CRUISE_SPEED, Handle, value);
        }

        public DrivingStyle DrivingStyle
        {
            set => Call(Hash.SET_DRIVE_TASK_DRIVING_STYLE, Handle, value);
        }

        public VehicleDrivingFlags VehicleDrivingFlags
        {
            set => Call(Hash.SET_DRIVE_TASK_DRIVING_STYLE, Handle, value);
        }

        #endregion

        #region Jacking

        public bool IsJacking => Call<bool>(Hash.IS_PED_JACKING, Handle);

        public bool IsBeingJacked => Call<bool>(Hash.IS_PED_BEING_JACKED, Handle);

        /// <summary>
        /// Sets a value indicating whether this <see cref="Ped"/> will stay in the vehicle when the driver gets jacked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if <see cref="Ped"/> stays in vehicle when jacked; otherwise, <see langword="false" />.
        /// </value>
        public bool StaysInVehicleWhenJacked
        {
            set => Call(Hash.SET_PED_STAY_IN_VEHICLE_WHEN_JACKED, Handle, value);
        }

        public Ped Jacker
        {
            get
            {
                var ped = new Ped(Call<int>(Hash.GET_PEDS_JACKER, Handle));
                return ped.Exists() ? ped : null;
            }
        }

        public Ped JackTarget
        {
            get
            {
                var ped = new Ped(Call<int>(Hash.GET_JACK_TARGET, Handle));
                return ped.Exists() ? ped : null;
            }
        }

        #endregion

        #region Parachuting

        public bool IsInParachuteFreeFall => Call<bool>(Hash.IS_PED_IN_PARACHUTE_FREE_FALL, Handle);

        public void OpenParachute()
        {
            Call(Hash.FORCE_PED_TO_OPEN_PARACHUTE, Handle);
        }

        public ParachuteState ParachuteState => Call<ParachuteState>(Hash.GET_PED_PARACHUTE_STATE, Handle);

        public ParachuteLandingType ParachuteLandingType =>
            Call<ParachuteLandingType>(Hash.GET_PED_PARACHUTE_LANDING_TYPE, Handle);

        #endregion

        #region Combat

        public bool IsEnemy
        {
            set => Call(Hash.SET_PED_AS_ENEMY, Handle, value);
        }

        public bool IsPriorityTargetForEnemies
        {
            set => Call(Hash.SET_ENTITY_IS_TARGET_PRIORITY, Handle, value, 0);
        }

        public bool IsFleeing => Call<bool>(Hash.IS_PED_FLEEING, Handle);

        /// <summary>
        /// Gets a value indicating whether this <see cref="Ped"/> is injured (<see cref="Entity.Health"/> of the <see cref="Ped"/> is lower than <see cref="InjuryHealthThreshold"/>) or does not exist.
        /// Can be called safely to check if <see cref="Ped"/>s exist and are not injured without calling <see cref="Exists"/>.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> this <see cref="Ped"/> is injured or does not exist; otherwise, <see langword="false" />.
        /// </value>
        /// <seealso cref="Entity.IsDead"/>
        /// <seealso cref="Exists"/>
        public bool IsInjured => Call<bool>(Hash.IS_PED_INJURED, Handle);

        public bool IsInStealthMode => Call<bool>(Hash.GET_PED_STEALTH_MOVEMENT, Handle);

        public bool IsInCombat => Call<bool>(Hash.IS_PED_IN_COMBAT, Handle);

        public bool IsInMeleeCombat => Call<bool>(Hash.IS_PED_IN_MELEE_COMBAT, Handle);

        public bool IsAiming => GetConfigFlag(78);

        public bool IsPlantingBomb => Call<bool>(Hash.IS_PED_PLANTING_BOMB, Handle);

        public bool IsShooting => Call<bool>(Hash.IS_PED_SHOOTING, Handle);

        public bool IsReloading => Call<bool>(Hash.IS_PED_RELOADING, Handle);

        public bool IsDoingDriveBy => Call<bool>(Hash.IS_PED_DOING_DRIVEBY, Handle);

        public bool IsGoingIntoCover => Call<bool>(Hash.IS_PED_GOING_INTO_COVER, Handle);

        public bool IsAimingFromCover => Call<bool>(Hash.IS_PED_AIMING_FROM_COVER, Handle);

        public bool IsBeingStunned => Call<bool>(Hash.IS_PED_BEING_STUNNED, Handle);

        public bool IsBeingStealthKilled => Call<bool>(Hash.IS_PED_BEING_STEALTH_KILLED, Handle);

        public bool IsPerformingStealthKill => Call<bool>(Hash.IS_PED_PERFORMING_STEALTH_KILL, Handle);

        public bool IsInCover => Call<bool>(Hash.IS_PED_IN_COVER, Handle, false);

        public bool IsInCoverFacingLeft => Call<bool>(Hash.IS_PED_IN_COVER_FACING_LEFT, Handle);

        public bool CanBeTargetted
        {
            get
            {
                var address = MemoryAddress;
                if (address == IntPtr.Zero || NativeMemory.PedDropsWeaponsWhenDeadOffset == 0)
                {
                    return false;
                }

                return !NativeMemory.IsBitSet(address + NativeMemory.PedDropsWeaponsWhenDeadOffset, 9);
            }
            set => Call(Hash.SET_PED_CAN_BE_TARGETTED, Handle, value);
        }

        public bool CanBeShotInVehicle
        {
            set => Call(Hash.SET_PED_CAN_BE_SHOT_IN_VEHICLE, Handle, value);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Ped"/> was killed by a stealth attack.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if this <see cref="Ped"/> was killed by stealth; otherwise, <see langword="false" />.
        /// </value>
        public bool WasKilledByStealth => Call<bool>(Hash.WAS_PED_KILLED_BY_STEALTH, Handle);

        /// <summary>
        /// Gets a value indicating whether this <see cref="Ped"/> was killed by a takedown.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this <see cref="Ped"/> was killed by a takedown; otherwise, <see langword="false" />.
        /// </value>
        public bool WasKilledByTakedown => Call<bool>(Hash.WAS_PED_KILLED_BY_TAKEDOWN, Handle);

        public Ped MeleeTarget
        {
            get
            {
                var ped = new Ped(Call<int>(Hash.GET_MELEE_TARGET_FOR_PED, Handle));
                return ped.Exists() ? ped : null;
            }
        }

        public bool IsInCombatAgainst(Ped target)
        {
            return Call<bool>(Hash.IS_PED_IN_COMBAT, Handle, target.Handle);
        }

        /// <summary>
        /// Gets the <see cref="Entity"/> that killed this <see cref="Ped"/>.
        /// </summary>
        public Entity Killer => FromHandle(Call<int>(Hash.GET_PED_SOURCE_OF_DEATH, Handle));

        /// <summary>
        /// Gets the <see cref="WeaponHash"/> that this <see cref="Ped"/> is killed with. The return value is not necessarily a weapon hash for a human <see cref="Ped"/>s (e.g. can be the hash of <c>WEAPON_COUGAR</c>).
        /// </summary>
        public WeaponHash CauseOfDeath => Call<WeaponHash>(Hash.GET_PED_CAUSE_OF_DEATH, Handle);

        /// <summary>
        /// Gets the time when this <see cref="Ped"/> is killed. This value determines how this <see cref="Ped"/> is rendered when <see cref="Game.IsThermalVisionActive"/> is <see langword="true" /> and the <see cref="Ped"/> is dead.
        /// </summary>
        public int TimeOfDeath => Call<int>(Hash.GET_PED_TIME_OF_DEATH, Handle);

        /// <summary>
        /// <para>Clears the <see cref="Entity"/> record that killed this <see cref="Ped"/>. Can be useful after resurrecting this <see cref="Ped"/>.</para>
        /// <para>Internally, when a <see cref="Ped"/> killed and the value for the source of death in the instance of this <see cref="Ped"/> is not <c>0</c> (not <see langword="null" />), the game does not write the memory address of the <see cref="Ped"/> that killed this <see cref="Ped"/>.</para>
        /// </summary>
        public void ClearKillerRecord()
        {
            var address = MemoryAddress;
            if (address == IntPtr.Zero || NativeMemory.PedSourceOfDeathOffset == 0)
            {
                return;
            }

            NativeMemory.WriteAddress(address + NativeMemory.PedSourceOfDeathOffset, IntPtr.Zero);
        }

        /// <summary>
        /// <para>Clears the record of the cause of death that killed this <see cref="Ped"/> with. Can be useful after resurrecting this <see cref="Ped"/>.</para>
        /// <para>Internally, when a <see cref="Ped"/> killed and the value for the cause of death in the instance of this <see cref="Ped"/> is not <c>0</c>, the game does not write the weapon hash value for the cause of death.</para>
        /// </summary>
        public void ClearCauseOfDeathRecord()
        {
            var address = MemoryAddress;
            if (address == IntPtr.Zero || NativeMemory.PedCauseOfDeathOffset == 0)
            {
                return;
            }

            NativeMemory.WriteInt32(address + NativeMemory.PedCauseOfDeathOffset, 0);
        }

        /// <summary>
        /// <para>Clears the time record when this <see cref="Ped"/> is killed. Can be useful after resurrecting this <see cref="Ped"/>.</para>
        /// <para>Internally, when a <see cref="Ped"/> killed and the value for the time of death in the instance of this <see cref="Ped"/> is not <c>0</c>, the game does not write the game time value for the time of death.</para>
        /// </summary>
        public void ClearTimeOfDeathRecord()
        {
            var address = MemoryAddress;
            if (address == IntPtr.Zero || NativeMemory.PedTimeOfDeathOffset == 0)
            {
                return;
            }

            NativeMemory.WriteInt32(address + NativeMemory.PedTimeOfDeathOffset, 0);
        }

        #endregion

        #region Damaging

        public bool CanWrithe
        {
            get => !GetConfigFlag(281);
            set => SetConfigFlag(281, !value);
        }

        /// <summary>
        /// Gets or Sets whether this <see cref="Ped"/> can suffer critical damage (which deals 1000 times base damages to non-player characters with default weapon configs) when bullets hit this <see cref="Ped"/>'s head bone or its child bones.
        ///  If this <see cref="Ped"/> can't suffer critical damage, they will take base damage of weapons when bullets hit their head bone or its child bones, just like when bullets hit a bone other than their head bone, its child bones, or limb bones.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if this <see cref="Ped"/> can suffer critical damage; otherwise, <see langword="false" />.
        /// </value>
        public bool CanSufferCriticalHits
        {
            get
            {
                var address = MemoryAddress;
                if (address == IntPtr.Zero || NativeMemory.PedSuffersCriticalHitOffset == 0)
                {
                    return false;
                }

                return !NativeMemory.IsBitSet(address + NativeMemory.PedSuffersCriticalHitOffset, 2);
            }
            set => Call(Hash.SET_PED_SUFFERS_CRITICAL_HITS, Handle, value);
        }

        public bool DiesOnLowHealth
        {
            set => Call(Hash.SET_PED_DIES_WHEN_INJURED, Handle, value);
        }

        public bool DiesInstantlyInWater
        {
            set => Call(Hash.SET_PED_DIES_INSTANTLY_IN_WATER, Handle, value);
        }

        public bool DrownsInWater
        {
            set => Call(Hash.SET_PED_DIES_IN_WATER, Handle, value);
        }

        public bool DrownsInSinkingVehicle
        {
            set => Call(Hash.SET_PED_DIES_IN_SINKING_VEHICLE, Handle, value);
        }

        /// <summary>
        /// Sets whether this <see cref="Ped"/> will drop the equipped weapon when they get killed.
        ///  Note that <see cref="Ped"/>s will drop only their equipped weapon when they get killed.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if <see cref="Ped"/> drops the equipped weapon when killed; otherwise, <see langword="false" />.
        /// </value>
        public bool DropsEquippedWeaponOnDeath
        {
            get
            {
                var address = MemoryAddress;
                if (address == IntPtr.Zero || NativeMemory.PedDropsWeaponsWhenDeadOffset == 0)
                {
                    return false;
                }

                return !NativeMemory.IsBitSet(address + NativeMemory.PedDropsWeaponsWhenDeadOffset, 14);
            }
            set => Call(Hash.SET_PED_DROPS_WEAPONS_WHEN_DEAD, Handle, value);
        }

        public void ApplyDamage(int damageAmount)
        {
            Call(Hash.APPLY_DAMAGE_TO_PED, Handle, damageAmount, true);
        }

        public override bool HasBeenDamagedBy(WeaponHash weapon)
        {
            return Call<bool>(Hash.HAS_PED_BEEN_DAMAGED_BY_WEAPON, Handle, weapon, 0);
        }

        public override bool HasBeenDamagedByAnyWeapon()
        {
            return Call<bool>(Hash.HAS_PED_BEEN_DAMAGED_BY_WEAPON, Handle, 0, 2);
        }

        public override bool HasBeenDamagedByAnyMeleeWeapon()
        {
            return Call<bool>(Hash.HAS_PED_BEEN_DAMAGED_BY_WEAPON, Handle, 0, 1);
        }

        public override void ClearLastWeaponDamage()
        {
            Call(Hash.CLEAR_PED_LAST_WEAPON_DAMAGE, Handle);
        }

        /// <summary>
        /// Gets or sets the injury health threshold for this <see cref="Ped"/>.
        /// The pedestrian is considered injured when its health drops below this value.
        /// The pedestrian dies on attacks when its health is below this value.
        /// </summary>
        /// <value>
        /// The injury health threshold. Should be below <see cref="Entity.MaxHealth"/>.
        /// </value>
        /// <remarks>
        /// Note on player controlled pedestrians: One of the game scripts will consider the player wasted when their health drops below this setting value.
        /// </remarks>
        public float InjuryHealthThreshold
        {
            get
            {
                var address = MemoryAddress;
                if (address == IntPtr.Zero || NativeMemory.InjuryHealthThresholdOffset == 0)
                {
                    return 0.0f;
                }

                return NativeMemory.ReadFloat(address + NativeMemory.InjuryHealthThresholdOffset);
            }
            set
            {
                var address = MemoryAddress;
                if (address == IntPtr.Zero || NativeMemory.InjuryHealthThresholdOffset == 0)
                {
                    return;
                }

                NativeMemory.WriteFloat(address + NativeMemory.InjuryHealthThresholdOffset, value);
            }
        }

        /// <summary>
        /// Gets or sets the fatal injury health threshold for this <see cref="Ped"/>.
        /// The pedestrian health will be set to 0.0 when it drops below this value.
        /// </summary>
        /// <value>
        /// The fatal injury health threshold. Should be below <see cref="Entity.MaxHealth"/>.
        /// </value>
        /// <remarks>
        /// Note on player controlled pedestrians: One of the game scripts will consider the player wasted when their health drops below <see cref="Ped.InjuryHealthThreshold"/>, regardless of this setting.
        /// </remarks>
        public float FatalInjuryHealthThreshold
        {
            get
            {
                var address = MemoryAddress;
                if (address == IntPtr.Zero || NativeMemory.FatalInjuryHealthThresholdOffset == 0)
                {
                    return 0.0f;
                }

                return NativeMemory.ReadFloat(address + NativeMemory.FatalInjuryHealthThresholdOffset);
            }
            set
            {
                var address = MemoryAddress;
                if (address == IntPtr.Zero || NativeMemory.FatalInjuryHealthThresholdOffset == 0)
                {
                    return;
                }

                NativeMemory.WriteFloat(address + NativeMemory.FatalInjuryHealthThresholdOffset, value);
            }
        }

        public Vector3 LastWeaponImpactPosition
        {
            get
            {
                Vector3 position;
                unsafe
                {
                    if (Call<bool>(Hash.GET_PED_LAST_WEAPON_IMPACT_COORD, Handle, &position))
                    {
                        return position;
                    }
                }

                return Vector3.Zero;
            }
        }

        #endregion

        #region Relationship

        public Relationship GetRelationshipWithPed(Ped ped)
        {
            return (Relationship)Call<int>(Hash.GET_RELATIONSHIP_BETWEEN_PEDS, Handle, ped.Handle);
        }

        public RelationshipGroup RelationshipGroup
        {
            get => new(Call<int>(Hash.GET_PED_RELATIONSHIP_GROUP_HASH, Handle));
            set => Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, Handle, value.Hash);
        }

        #endregion

        #region Perception

        public float SeeingRange
        {
            get
            {
                if (NativeMemory.SeeingRangeOffset == 0)
                    return 0.0f;

                var address = PedIntelligenceAddress;
                if (address == IntPtr.Zero)
                    return 0.0f;

                return NativeMemory.ReadFloat(address + NativeMemory.SeeingRangeOffset);
            }
            set => Call(Hash.SET_PED_SEEING_RANGE, Handle, value);
        }

        public float HearingRange
        {
            get
            {
                if (NativeMemory.HearingRangeOffset == 0)
                    return 0.0f;

                var address = PedIntelligenceAddress;
                if (address == IntPtr.Zero)
                    return 0.0f;

                return NativeMemory.ReadFloat(address + NativeMemory.HearingRangeOffset);
            }
            set => Call(Hash.SET_PED_HEARING_RANGE, Handle, value);
        }

        public float VisualFieldMinAngle
        {
            get
            {
                if (NativeMemory.VisualFieldMinAngleOffset == 0)
                    return 0.0f;

                var address = PedIntelligenceAddress;
                if (address == IntPtr.Zero)
                    return 0.0f;

                return NativeMemory.ReadFloat(address + NativeMemory.VisualFieldMinAngleOffset);
            }
            set => Call(Hash.SET_PED_VISUAL_FIELD_MIN_ANGLE, Handle, value);
        }

        public float VisualFieldMaxAngle
        {
            get
            {
                if (NativeMemory.VisualFieldMaxAngleOffset == 0)
                    return 0.0f;

                var address = PedIntelligenceAddress;
                if (address == IntPtr.Zero)
                    return 0.0f;

                return NativeMemory.ReadFloat(address + NativeMemory.VisualFieldMaxAngleOffset);
            }
            set => Call(Hash.SET_PED_VISUAL_FIELD_MAX_ANGLE, Handle, value);
        }

        public float VisualFieldMinElevationAngle
        {
            get
            {
                if (NativeMemory.VisualFieldMinElevationAngleOffset == 0)
                    return 0.0f;

                var address = PedIntelligenceAddress;
                if (address == IntPtr.Zero)
                    return 0.0f;

                return NativeMemory.ReadFloat(address + NativeMemory.VisualFieldMinElevationAngleOffset);
            }
            set => Call(Hash.SET_PED_VISUAL_FIELD_MIN_ELEVATION_ANGLE, Handle, value);
        }

        public float VisualFieldMaxElevationAngle
        {
            get
            {
                if (NativeMemory.VisualFieldMaxElevationAngleOffset == 0)
                    return 0.0f;

                var address = PedIntelligenceAddress;
                if (address == IntPtr.Zero)
                    return 0.0f;

                return NativeMemory.ReadFloat(address + NativeMemory.VisualFieldMaxElevationAngleOffset);
            }
            set => Call(Hash.SET_PED_VISUAL_FIELD_MAX_ELEVATION_ANGLE, Handle, value);
        }

        public float VisualFieldPeripheralRange
        {
            get
            {
                if (NativeMemory.VisualFieldPeripheralRangeOffset == 0)
                    return 0.0f;

                var address = PedIntelligenceAddress;
                if (address == IntPtr.Zero)
                    return 0.0f;

                return NativeMemory.ReadFloat(address + NativeMemory.VisualFieldPeripheralRangeOffset);
            }
            set => Call(Hash.SET_PED_VISUAL_FIELD_PERIPHERAL_RANGE, Handle, value);
        }

        public float VisualFieldCenterAngle
        {
            get
            {
                if (NativeMemory.VisualFieldCenterAngleOffset == 0)
                    return 0.0f;

                var address = PedIntelligenceAddress;
                if (address == IntPtr.Zero)
                    return 0.0f;

                return NativeMemory.ReadFloat(address + NativeMemory.VisualFieldCenterAngleOffset);
            }
            set => Call(Hash.SET_PED_VISUAL_FIELD_CENTER_ANGLE, Handle, value);
        }

        #endregion

        #region Group

        /// <summary>
        /// Gets if this <see cref="Ped"/> is in a <see cref="PedGroup"/>.
        /// </summary>
        public bool IsInGroup => Call<bool>(Hash.IS_PED_IN_GROUP, Handle);

        public void LeaveGroup()
        {
            Call(Hash.REMOVE_PED_FROM_GROUP, Handle);
        }

        public bool NeverLeavesGroup
        {
            set => Call(Hash.SET_PED_NEVER_LEAVES_GROUP, Handle, value);
        }

        /// <summary>
        /// Gets the PedGroup this <see cref="Ped"/> is in.
        /// </summary>
        public PedGroup PedGroup => IsInGroup ? new PedGroup(Call<int>(Hash.GET_PED_GROUP_INDEX, Handle, false)) : null;

        #endregion

        #region Speech & Animation

        public bool CanPlayGestures
        {
            set => Call(Hash.SET_PED_CAN_PLAY_GESTURE_ANIMS, Handle, value);
        }

        public bool IsPainAudioEnabled
        {
            set => Call(Hash.DISABLE_PED_PAIN_AUDIO, Handle, !value);
        }

        public bool IsAmbientSpeechPlaying => Call<bool>(Hash.IS_AMBIENT_SPEECH_PLAYING, Handle);

        public bool IsScriptedSpeechPlaying => Call<bool>(Hash.IS_SCRIPTED_SPEECH_PLAYING, Handle);

        public bool IsAnySpeechPlaying => Call<bool>(Hash.IS_ANY_SPEECH_PLAYING, Handle);

        public bool IsAmbientSpeechEnabled => !Call<bool>(Hash.IS_AMBIENT_SPEECH_DISABLED, Handle);

        public void PlayAmbientSpeech(string speechName, SpeechModifier modifier = SpeechModifier.Standard)
        {
            if (modifier < 0 || (int)modifier >= _speechModifierNames.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(modifier));
            }

            Call(Hash.PLAY_PED_AMBIENT_SPEECH_NATIVE, Handle, speechName, _speechModifierNames[(int)modifier]);
        }

        public void PlayAmbientSpeech(string speechName, string voiceName,
            SpeechModifier modifier = SpeechModifier.Standard)
        {
            if (modifier < 0 || (int)modifier >= _speechModifierNames.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(modifier));
            }

            Call(Hash.PLAY_PED_AMBIENT_SPEECH_WITH_VOICE_NATIVE, Handle, speechName, voiceName,
                _speechModifierNames[(int)modifier], 0);
        }

        /// <summary>
        /// Sets the voice to use when this <see cref="Ped"/> speaks.
        /// </summary>
        public string Voice
        {
            set => Call(Hash.SET_AMBIENT_VOICE_NAME, Handle, value);
        }

        /// <summary>
        /// Sets the animation dictionary or set this <see cref="Ped"/> should use or <see langword="null" /> to clear it.
        /// </summary>
        public string MovementAnimationSet
        {
            set
            {
                if (value == null)
                {
                    Call(Hash.RESET_PED_MOVEMENT_CLIPSET, 0.25f);
                    Task.ClearAll();
                }
                else
                {
                    // Movement sets can be applied from animation dictionaries and animation sets (also clip sets but they use the same native as animation sets).
                    // So check if the string is a valid dictionary, if so load it as such. Otherwise load it as an animation set.
                    bool isDict = Call<bool>(Hash.DOES_ANIM_DICT_EXIST, value);

                    Call(isDict ? Hash.REQUEST_ANIM_DICT : Hash.REQUEST_ANIM_SET, value);
                    var endtime = DateTime.UtcNow + new TimeSpan(0, 0, 0, 0, 1000);

                    while (!Call<bool>(isDict ? Hash.HAS_ANIM_DICT_LOADED : Hash.HAS_ANIM_SET_LOADED, value))
                    {
                        Script.Yield();

                        if (DateTime.UtcNow >= endtime)
                        {
                            return;
                        }
                    }

                    Call(Hash.SET_PED_MOVEMENT_CLIPSET, value, 0.25f);
                }
            }
        }

        #endregion

        public static PedHash[] GetAllModels()
        {
            return NativeMemory.PedModels.Select(x => (PedHash)x).ToArray();
        }

        /// <summary>
        /// Gets an <c>array</c> of all loaded <see cref="PedHash"/>s that is appropriate to spawn as ambient vehicles.
        /// The result array can contains animal hashes, which CREATE_RANDOM_PED excludes to spawn.
        /// All the model hashes of the elements are loaded and the <see cref="Ped"/>s with the model hashes can be spawned immediately.
        /// </summary>
        public static PedHash[] GetAllLoadedModelsAppropriateForAmbientPeds()
        {
            return NativeMemory.GetLoadedAppropriatePedHashes()
                .Select(x => (PedHash)x)
                .ToArray();
        }
    }
}