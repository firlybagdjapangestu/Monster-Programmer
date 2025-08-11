using UnityEngine;
using UnityEngine.Video;

namespace Data
{
    [CreateAssetMenu(fileName = "New Material Study", menuName = "Materi/New Materi")]
    public class MaterialStudy : ScriptableObject
    {
        [Header("[Indentity]")]
        [SerializeField] private MapMonster typeSubject = MapMonster.Encapsulation;
        [SerializeField] private VideoClip clip;

        [Header("[Data]")]
        [SerializeField] private string[] allSubject = new string[0];


        public MapMonster TypeSubject => typeSubject;
        public string[] AllSubject => allSubject;
        public VideoClip Clip => clip;
    }

}

