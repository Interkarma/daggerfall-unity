using UnityEngine;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;

namespace Game.Pet
{
    public class PetEntity : EnemyEntity
    {
        private int _careerIndex = -1;

        public PetEntity(DaggerfallEntityBehaviour entityBehaviour)
            : base(entityBehaviour)
        {
        }

        public override void SetEntityDefaults()
        {
        }

        public void SetCareer(MobileEnemy mobileEnemy, EntityTypes entityType)
        {
            career = GetCustomCareerTemplate(mobileEnemy.ID);

            if (career != null)
            {
                _careerIndex = mobileEnemy.ID;
                stats.SetPermanentFromCareer(career);

                if (entityType == EntityTypes.EnemyMonster)
                {
                    level = mobileEnemy.Level;
                    maxHealth = Random.Range(mobileEnemy.MinHealth, mobileEnemy.MaxHealth + 1);
                    for (int i = 0; i < ArmorValues.Length; i++)
                    {
                        ArmorValues[i] = (sbyte) (mobileEnemy.ArmorValue * 5);
                    }
                }
                else
                {
                    level = GameManager.Instance.PlayerEntity.Level;
                    maxHealth = FormulaHelper.RollEnemyClassMaxHealth(level, career.HitPointsPerLevel);
                }
            }
            else if (entityType == EntityTypes.EnemyMonster)
            {
                _careerIndex = mobileEnemy.ID;
                career = GetMonsterCareerTemplate((MonsterCareers) _careerIndex);
                stats.SetPermanentFromCareer(career);
                level = mobileEnemy.Level;
                maxHealth = Random.Range(mobileEnemy.MinHealth, mobileEnemy.MaxHealth + 1);
                for (int i = 0; i < ArmorValues.Length; i++)
                {
                    ArmorValues[i] = (sbyte) (mobileEnemy.ArmorValue * 5);
                }
            }
            else if (entityType == EntityTypes.EnemyClass)
            {
                _careerIndex = mobileEnemy.ID - 128;
                career = GetClassCareerTemplate((ClassCareers) _careerIndex);
                stats.SetPermanentFromCareer(career);

                // Enemy class is levelled to player and uses similar health rules
                // City guards are 3 to 6 levels above the player
                level = GameManager.Instance.PlayerEntity.Level;
                if (_careerIndex == (int) MobileTypes.Knight_CityWatch - 128)
                    level += UnityEngine.Random.Range(3, 7);

                maxHealth = FormulaHelper.RollEnemyClassMaxHealth(level, career.HitPointsPerLevel);
            }
            else
            {
                career = new DFCareer();
                _careerIndex = -1;
                return;
            }

            name = career.Name;
            minMetalToHit = mobileEnemy.MinMetalToHit;
            team = mobileEnemy.Team;

            var skillsLevel = (short) ((level * 5) + 30);

            if (skillsLevel > 100) skillsLevel = 100;

            for (int i = 0; i <= DaggerfallSkills.Count; i++)
            {
                skills.SetPermanentSkillValue(i, skillsLevel);
            }

            FillVitalSigns();
        }
    }
}