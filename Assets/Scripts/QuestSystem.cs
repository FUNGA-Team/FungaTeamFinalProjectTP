using System.Collections;

using System.Collections.Generic;

using UnityEngine;

namespace AC

{

    public class QuestSystem : MonoBehaviour

    {
        #region Variables

        private Quest[] quests;

        [SerializeField]
        private int stringVariableID;

        #endregion

        #region UnityStandards

        private void OnEnable()

        {
            DontDestroyOnLoad(gameObject);

            GenerateQuests();

            EventManager.OnObjectiveUpdate += OnObjectiveUpdate;
        }

        private void OnDisable()

        {
            EventManager.OnObjectiveUpdate -= OnObjectiveUpdate;
        }

        #endregion

        #region PrivateFunctions

        private void GenerateQuests()

        {
            List<Quest> questsList = new List<Quest>();

            foreach (Objective objective in KickStarter.inventoryManager.objectives)

            {
                if (!objective.Title.Contains("/"))

                {
                    Quest quest = new Quest(objective);

                    questsList.Add(quest);
                }
            }

            quests = questsList.ToArray();
        }

        private void OnObjectiveUpdate(Objective objective, ObjectiveState state)

        {
            foreach (Quest quest in quests)

            {
                quest.OnObjectiveUpdate(objective.ID);
            }

            UpdateActiveQuestsString();
        }

        private void UpdateActiveQuestsString()

        {
            GVar stringVariable = GlobalVariables.GetVariable(stringVariableID);
            Debug.Log("Variable 111" + stringVariable);
            if (stringVariable == null)

            {
                return;
            }

            string result = string.Empty;

            foreach (Quest quest in quests)

            {
                result += quest.GetActiveString();

                result += "\n";
            }

            stringVariable.SetStringValue(result);
        }

        #endregion

        [System.Serializable]

        private class Quest

        {
            #region Variables

            private int mainObjectiveID;

            private List<int> subObjectiveIDs = new List<int>();

            #endregion

            #region Constructors

            public Quest(Objective mainObjective)

            {
                mainObjectiveID = mainObjective.ID;

                subObjectiveIDs = new List<int>();

                string requiredPrefix = mainObjective.Title + "/";

                foreach (Objective objective in KickStarter.inventoryManager.objectives)

                {
                    if (objective != mainObjective &&
                        objective.Title.StartsWith(requiredPrefix))

                    {
                        // Is a sub-objective

                        subObjectiveIDs.Add(objective.ID);
                    }
                }
            }

            #endregion

            #region PublicFunctions

            public void OnObjectiveUpdate(int objectiveID)

            {
                if (!subObjectiveIDs.Contains(objectiveID) ||

                    !IsValid())

                {
                    return;
                }

                AutoUpdateMainState();
            }

            public string GetActiveString()

            {
                if (!IsValid())

                {
                    return string.Empty;
                }

                ObjectiveInstance mainObjectiveInstance =
                    KickStarter.runtimeObjectives.GetObjective(mainObjectiveID);

                if (mainObjectiveInstance == null ||
                    mainObjectiveInstance.CurrentState.stateType !=
                        ObjectiveStateType.Active)

                {
                    return string.Empty;
                }

                int language = Options.GetLanguage();

                string result =
                    "<b>" + mainObjectiveInstance.Objective.GetTitle(language) + ":</b>";

                result += "\n";

                foreach (int subObjectiveID in subObjectiveIDs)

                {
                    ObjectiveInstance subObjectiveInstance =
                        KickStarter.runtimeObjectives.GetObjective(subObjectiveID);

                    if (subObjectiveInstance == null)

                    {
                        continue;
                    }

                    result += " - " + subObjectiveInstance.Objective.GetTitle(language);

                    switch (subObjectiveInstance.CurrentState.stateType)

                    {
                        case ObjectiveStateType.Fail:

                            result += " (FAILED)";

                            break;

                        case ObjectiveStateType.Complete:

                            result += " (COMPLETE)";

                            break;

                        default:

                            break;
                    }

                    result += "\n";
                }

                return result;
            }

            #endregion

            #region PrivateFunctions

            private void AutoUpdateMainState()

            {
                int completedSubObjectives = 0;

                int failedSubObjectives = 0;

                int activeSubObjectives = 0;

                foreach (int subObjectiveID in subObjectiveIDs)

                {
                    ObjectiveState subObjectiveState =
                        KickStarter.runtimeObjectives.GetObjectiveState(subObjectiveID);

                    if (subObjectiveState == null)

                    {
                        continue;
                    }

                    switch (subObjectiveState.stateType)

                    {
                        case ObjectiveStateType.Active:

                            activeSubObjectives++;

                            break;

                        case ObjectiveStateType.Complete:

                            completedSubObjectives++;

                            break;

                        case ObjectiveStateType.Fail:

                            failedSubObjectives++;

                            break;

                        default:

                            break;
                    }
                }

                int totalSubObjectives = subObjectiveIDs.Count;

                if (completedSubObjectives == totalSubObjectives)

                {
                    // Completed all sub-objectives, main is completed

                    KickStarter.runtimeObjectives.SetObjectiveState(
                        mainObjectiveID, ObjectiveStateType.Complete);

                }

                else if (failedSubObjectives > 0)

                {
                    // Failed a sub-objectives, main is failed

                    KickStarter.runtimeObjectives.SetObjectiveState(
                        mainObjectiveID, ObjectiveStateType.Fail);

                }

                else if (activeSubObjectives > 0)

                {
                    // Main is active

                    KickStarter.runtimeObjectives.SetObjectiveState(
                        mainObjectiveID, ObjectiveStateType.Active);

                }

                else

                {
                    // Main is inactive
                }
            }

            private bool IsValid()

            {
                if (mainObjectiveID < 0 ||

                    subObjectiveIDs.Contains(mainObjectiveID) ||

                    subObjectiveIDs.Count == 0)

                {
                    return false;
                }

                return true;
            }

            #endregion
        }
    }

}