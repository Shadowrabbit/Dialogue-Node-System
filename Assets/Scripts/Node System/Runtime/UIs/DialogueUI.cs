using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.NodeSystem
{
    public class DialogueUI : MonoBehaviour
    {

        #region Fields


        #region Dialogue


        [Space(10)]
        [Header("General :")]
        [Space(10)]

        [SerializeField, Tooltip("Ecrire caract�re par caract�re ou afficher la r�plique enti�re d'un coup ?")]
        protected bool writeCharByChar = true;

        [SerializeField, Tooltip("Vitesse d'�criture caract�re par caract�re.")]
        protected float charWriteSpeed = .1f;

        [SerializeField, Tooltip("Drapeau qui indique si le UI est en train d'�crire la r�plique en cours.")]
        protected bool isWriting;

        [SerializeField] protected GameObject dialogueUI;

        #endregion


        #region UIs



        #region Dialogue Characters

        [Space(10)]
        [Header("Dialogue Characters :")]
        [Space(10)]

        [SerializeField] protected GameObject characterLeftGo;
        [SerializeField] protected GameObject characterRightGo;
        [SerializeField] protected Image characterLeftImg;
        [SerializeField] protected Image characterRightImg;
        [SerializeField] protected TextMeshProUGUI characterNameText;

        #endregion


        #region Dialogue Text

        [Space(10)]
        [Header("Dialogue Text :")]
        [Space(10)]

        [SerializeField] protected GameObject dialogueContent;
        [SerializeField] protected Button continueBtn;
        [SerializeField] protected Button skipRepliqueBtn;
        [SerializeField] protected TextMeshProUGUI dialogueText;
        [SerializeField] protected TextMeshProUGUI continueBtnText;
        protected StringBuilder sb;  //Pour afficher les caract�res du texte un � la fois
        #endregion


        #region Dialogue Choices

        [Space(10)]
        [Header("Dialogue Choices :")]
        [Space(10)]

        [SerializeField] protected GameObject choicesContent;

        [SerializeField] protected Button[] choiceBtns;   //Rempli une seule fois par code pour y acc�der plus rapidement
        [SerializeField] protected TextMeshProUGUI[] choiceTmps;   //Rempli une seule fois par code pour y acc�der plus rapidement

        #endregion


        #region Audio

        [Space(10)]
        [Header("Audio :")]
        [Space(10)]

        protected AudioSource source;

        #endregion


        #endregion



        #endregion




        #region Mono

        void Awake()
        {
            source = gameObject.AddComponent<AudioSource>();
            choicesContent.SetActive(false);
            dialogueContent.SetActive(false);
        }


        #endregion


        #region Dialogue

        public void StartDialogue()
        {
            //Quand on lance le dialogue, on active l'interface
            dialogueUI.SetActive(true);
        }

        public void EndDialogue()
        {
            dialogueUI.SetActive(false);
        }




        //Quand on clique sur le bouton continuer, on afficher le panel des choix 
        //s'il y a des ports, sinon, on joue la node suivante
        public void SetContinueBtn(UnityAction onContinueClicked)
        {
            continueBtn.onClick.RemoveAllListeners(); 
            continueBtn.onClick.AddListener(onContinueClicked);
        }


        //Quand l'UI doit afficher des choix, le bouton continuer appelle cette fonction
        public void ShowChoicesPanel()
        {
            dialogueContent.SetActive(false);
            choicesContent.SetActive(true);
        }



        //Quand un choix est s�lectionn�, ramen� la fen�tre de la r�plique
        public void OnChoiceSelected()
        {
            dialogueContent.SetActive(true);
            choicesContent.SetActive(false);
        }



        public void SetText(string replique)
        {
            dialogueContent.SetActive(true);
            skipRepliqueBtn.onClick.RemoveAllListeners();   

            if (writeCharByChar)
            {
                skipRepliqueBtn.onClick.AddListener(() => WriteAllText(replique));    //On assigne au bouton skip sa fonction
                StartCoroutine(WriteRepliqueCharByCharCo(replique));
            }
            else
            {
                dialogueText.text = replique;

                skipRepliqueBtn.gameObject.SetActive(false);
                continueBtnText.gameObject.SetActive(true);
                continueBtn.interactable = true;
            }
        }

        private void WriteAllText(string replique)
        {
            //On n'est plus en train d'�crire, donc on remet les valeurs par d�faut
            skipRepliqueBtn.gameObject.SetActive(false);
            continueBtnText.gameObject.SetActive(true);
            continueBtn.interactable = true;
            isWriting = false;

            //On arr�te la coroutine pour �viter d'�crire � la suite de la r�plique enti�re
            StopAllCoroutines();
            dialogueText.text = replique;
        }


        //Pour �crire la r�plique caract�re par caract�re � l'aide du StringBuilder
        private IEnumerator WriteRepliqueCharByCharCo(string replique)
        {
            skipRepliqueBtn.gameObject.SetActive(true);
            continueBtnText.gameObject.SetActive(false);
            continueBtn.interactable = false;   //Pour �viter de cliquer dessus, on sait jamais
            isWriting = true;

            int length = replique.Length;
            sb = new StringBuilder(length);
            WaitForSeconds wait = new WaitForSeconds(charWriteSpeed);

            for (int i = 0; i < length; i++)
            {
                sb.Append(replique[i]);
                dialogueText.text = sb.ToString();
                //Wait a certain amount of time, then continue with the for loop
                yield return wait;
            }

            isWriting = false;
            continueBtn.interactable = true;   //Pour �viter de cliquer dessus, on sait jamais
            continueBtnText.gameObject.SetActive(true);
            skipRepliqueBtn.gameObject.SetActive(false);
        }


        public void SetCharacter(string characterName, Color characterNameColor, Sprite characterSprite, DialogueSide faceDir, DialogueSide sidePlacement)
        {
            if (sidePlacement == DialogueSide.Left)
            {
                //S'il n'y a pas de sprite en param�tre, on veut cacher le personnage de ce c�t�
                if (!characterSprite)
                {
                    characterLeftGo.gameObject.SetActive(false);
                    return;
                }
                else
                {
                    characterLeftGo.gameObject.SetActive(true); 

                }

                characterLeftImg.sprite = characterSprite;

            }
            else
            {
                //S'il n'y a pas de sprite en param�tre, on veut cacher le personnage de ce c�t�
                if (!characterSprite)
                {
                    characterRightGo.gameObject.SetActive(false);
                    return;
                }
                else
                {
                    characterRightGo.gameObject.SetActive(true);
                }

                characterRightImg.sprite = characterSprite;

            }


            characterNameText.text = characterName;
            characterNameText.color = characterNameColor;
        }


        public void SetChoices(List<DialogueButtonContainer> dialogueButtonContainers)
        {
            choicesContent.SetActive(true);

            for (int i = 0; i < choiceBtns.Length; i++)
            {
                Button b = choiceBtns[i];

                //Si on a des boutons en trop, on les d�sactive
                if (i >= dialogueButtonContainers.Count)
                {
                    b.gameObject.SetActive(false);
                }
                else
                {
                    //Si les conditions sont r�unies, on active le bouton normalement
                    if (dialogueButtonContainers[i].ConditionCheck)
                    {
                        b.gameObject.SetActive(true);
                        b.interactable = true;

                        b.onClick.RemoveAllListeners();
                        b.onClick.AddListener(dialogueButtonContainers[i].OnChoiceClicked);
                        b.onClick.AddListener(OnChoiceSelected);

                        choiceTmps[i].text = dialogueButtonContainers[i].Text;
                    }
                    else
                    {
                        //On n'active le bouton que si on doit le griser, puis on met son interactable � false
                        //pour montrer au joueur le choix indisponible en gris
                        b.gameObject.SetActive(dialogueButtonContainers[i].ChoiceState == ChoiceStateType.GreyOut);
                        b.interactable = false;

                        choiceTmps[i].text = dialogueButtonContainers[i].Text;
                    }


                }
            }
        }


        public void PlaySound(AudioClip clip)
        {
            source.Stop();
            source.clip = clip;
            source.Play();
        }

        #endregion
    }
}