using System;
using System.Collections.Generic;
using System.Linq;
using KimLIb.ModuleSystems;
using Member.KYM.Scripts.Agents.FSM;
using Member.KYM.Scripts.Players;
using Member.KYM.Scripts.Players.FSM;
using Member.KYM.Scripts.Players.FSM.Interface;
using UnityEngine;

namespace Member.KYM.Scripts.CombatSystems
{
    public class PlayerSkillModule : MonoBehaviour, IModule, ISkillModule, IAfterInitModule
    {
        public ModuleOwner Owner { get; private set; }
        public PlayerController Player { get; private set; }
        public event Action OnCurrentSkillEnd;

        private Dictionary<int, ISkill> _skillDict;
        private ISkill _currentSkill;
        
        public void Initialize(ModuleOwner owner)
        {
            Owner = owner;
            Player = owner as PlayerController;
            Debug.Assert(Player != null, "플레이어 스킬 모듈은 플레이어의 자식으로 존재해야 합니다.");

            _skillDict = GetComponentsInChildren<ISkill>()
                .ToDictionary(skill => skill.SkillData.skillIndex);
            
            foreach (ISkill skill in _skillDict.Values)
                skill.InitializeSkill(this);
        }

        public void AfterInit()
        {
            Player.PlayerInput.OnAttackKeyPressed += HandleAttackKeyPressed;
        }


        private void OnDestroy()
        {
            if (Player != null && Player.PlayerInput != null)
            {
                Player.PlayerInput.OnAttackKeyPressed -= HandleAttackKeyPressed;
            }
        }
        private void HandleAttackKeyPressed()
        {
            if (Player.CombatMode == PlayerCombatModes.NORMAL) return;
            
            if (Player.StateMachine.CurrentState is PlayerRunState)
            {
                if (CanUseSkill(2) && Player.StateMachine.CurrentState is ICanAttackState)
                {
                    if (_currentSkill is not { IsUsing: true })
                        Player.ChangeState(PlayerStateEnum.SKILL, 0);

                    UseSkill(2);
                }
            }
            else if (Player.PlayerInput.IsShiftPress)
            {
                if (CanUseSkill(1) && Player.StateMachine.CurrentState is ICanAttackState)
                {
                    if (_currentSkill is not { IsUsing: true })
                        Player.ChangeState(PlayerStateEnum.SKILL, 0);

                    UseSkill(1);
                }
            }
            if (CanUseSkill(0) && Player.StateMachine.CurrentState is ICanAttackState)
            {
                if (_currentSkill is not { IsUsing: true })
                    Player.ChangeState(PlayerStateEnum.SKILL, 0);

                UseSkill(0);
            }
        }

        public bool CanUseSkill(int skillIndex, GameObject target = null)
        {
            if (_skillDict.TryGetValue(skillIndex, out ISkill skill))
            {
                if (_currentSkill is { IsUsing: true } && _currentSkill != skill)
                    return false;

                return skill.CanUseSkill(target);
            }

            return false;
        }

        public void UseSkill(int skillIndex, GameObject target = null)
        {
            if (_skillDict.TryGetValue(skillIndex, out ISkill skill))
            {
                if (_currentSkill != skill)
                {
                    if (_currentSkill != null)
                        _currentSkill.OnSkillEnd -= HandleCurrentSkillEnd;

                    _currentSkill = skill;
                    _currentSkill.OnSkillEnd += HandleCurrentSkillEnd;
                }

                _currentSkill.UseSkill(target);
            }
        }

        private void HandleCurrentSkillEnd()
        {
            _currentSkill.OnSkillEnd -= HandleCurrentSkillEnd;
            InvokeSkillEnd();
            _currentSkill = null;
        }

        public void InvokeSkillEnd() => OnCurrentSkillEnd?.Invoke();
        
        public void StopSkillIfNotFinished()
        {
            if (_currentSkill != null)
            {
                _currentSkill.StopSkill();
                _currentSkill = null;
            }
        }
    }
}
