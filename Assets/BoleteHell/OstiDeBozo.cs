using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace BoleteHell
{
    public class OstiDeBozo : MonoBehaviour
    {
        public TextMeshPro textMeshPro;
        
        private void Start()
        {
            DisplayRandomText();
        }

        private void Update()
        {
            // every 5 seconds, display a new random text
            if (Time.time % 3 < Time.deltaTime)
            {
                DisplayRandomText();
            }
        }

        private void DisplayRandomText()
        {
            string randomText = GetRandomText();
            textMeshPro.text = randomText;
        }
        
        private int health = 100;
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                if (health <= 0)
                    Destroy(gameObject);

                health -= 5;
            }
        }

        private readonly string[] texts = 
        {
            "L'heure de ta défaite a sonnée",
            "Il est temps de mourir",
            "Je vais te défoncer",
            "Tu n'as aucune chance",
            "Je vais te pourfendre!",
            "La fin approche pour toi",
            "Ta défaite est inévitable",
            "Tu vas mordre la poussière",
            "Ma vengeance sera terrible",
            "Tu vas payer ton insolence avec ton sang",
            "Je vais te faire regretter d'être venu ici",
            "Rien ne peut te sauver maintenant",
            "Seul le désespoir t'attend",
            "Dieu m'a donné la force de te vaincre",
            "Je suis le cauchemar qui hante tes nuits",
            "Je suis la tempête qui va te balayer",
            "En taro Artanis!",
            "Je suis la mort, je suis la fin",
            "Je suis ton jugement dernier",
            "Je suis le fléau de ta vie",
            "Je suis la fin de ton histoire",
            "Je suis le bourreau de ton destin",
            "Je suis le serpent qui va te mordre",
            "Je suis le feu qui va te consumer",
            "Je suis la fureur qui va te détruire",
            "Je suis la colère qui va te ronger",
            "Prier ne te sauvera pas",
            "Ne cherche pas à fuir",
            "Il n'y a pas d'échappatoire",
            "Tu es tombé dans mon piège",
            "Ton brainrot t'aura coûté cher",
            "Défends-toi si tu peux, peureux",
            "Il n'y a pas de répit pour les lâches",
            "Je suis le châtiment qui va te frapper",
            "Le roi Alphée ne peut pas te sauver",
        };
    
        private string GetRandomText()
        {
            int randomIndex = UnityEngine.Random.Range(0, texts.Length);
            return texts[randomIndex];
        }
    }
}