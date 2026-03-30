using System.Collections.Generic;
using UnityEngine;

namespace Deviloop
{
    [CreateAssetMenu(fileName = "Stats_Enemy_", menuName = "Scriptable Objects/Combat/EnemyData", order = 1)]
    public class EnemyData : CombatCharacterStats
    {
        public Enemy prefab;

        public List<EnemyActionProbability> enemyActions;

        [DeveloperNotes, SerializeField]
        private string _developerNotes;

        [System.Serializable]
        public class EnemyActionProbability
        {
            [SerializeField, SerializeReference, SubclassSelector]
            public EnemyAction Action;
            public int Probability;
        }

        private int ActionsTotalWeight()
        {
            int weight = 0;
            foreach (EnemyActionProbability action in enemyActions)
            {
                weight += action.Probability;
            }
            return weight;
        }

        public EnemyAction GetNextAction(EnemyAction previousAction)
        {
            int totalWeight = ActionsTotalWeight();
            int randomIndex = 0;
            int randomValue = SeededRandom.Range(0, totalWeight);
            int cumulativeWeight = 0;

            for (int i = 0; i < enemyActions.Count; i++)
            {
                cumulativeWeight += enemyActions[i].Probability;
                if (randomValue < cumulativeWeight)
                {
                    randomIndex = i;
                    break;
                }
            }

            EnemyAction nextAction = enemyActions[randomIndex].Action;
            if (nextAction.CanBeTaken(previousAction) == false)
            {
                return GetNextAction(previousAction);
            }

            nextAction.OnActionSelected();
            return nextAction;
        }
    }
}
