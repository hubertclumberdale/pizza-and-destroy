using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    public GameObject demoRoom;
    public GameObject roomPrefab;
    public List<Room> rooms = new List<Room>();
    public float countDown = 5f;
    public float timeRemaining = 10;
    public Text timerUI;
    public int winMoney = 500;
    public int loseMoney = -1000;
    public Player player;
    private bool gameStarted = false;
    private bool gameOver = false;
    private bool playedMetalMusic = false;
    private AudioSource audioSource;
    public AudioClip softMusicIntro;
    public AudioClip softMusicLoop;
    public AudioClip metalMusicIntro;
    public AudioClip metalMusicLoop;

    public AudioClip gameOverMusicIntro;
    public AudioClip gameOverMusicLoop;

    public AudioClip winMusic;
    public Text walletText;

    public Text winText;
    public Text loseText;
    public Text destructionText;

    void Start()
    {
        Application.targetFrameRate = 60;
        InstanceFirstRoom();
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        StartCoroutine(PlaySoftMusic());
    }

    IEnumerator PlaySoftMusic()
    {
        audioSource.clip = softMusicIntro;
        audioSource.Stop();
        audioSource.Play();
        yield return new WaitForSeconds(softMusicIntro.length);
        if(!playedMetalMusic){
            audioSource.Stop();
            audioSource.clip = softMusicLoop;
            audioSource.Play();
        }
    }
    IEnumerator PlayMetalMusic()
    {
        playedMetalMusic = true;
        StopCoroutine(PlaySoftMusic());
        audioSource.clip = metalMusicIntro;
        audioSource.Stop();
        audioSource.Play();
        yield return new WaitForSeconds(metalMusicIntro.length);
        audioSource.Stop();
        audioSource.clip = metalMusicLoop;
        audioSource.Play();
    }
    IEnumerator PlayGameOverMusic()
    {
        audioSource.clip = gameOverMusicIntro;
        audioSource.Stop();
        audioSource.Play();
        yield return new WaitForSeconds(gameOverMusicIntro.length);
        audioSource.Stop();
        audioSource.clip = gameOverMusicLoop;
        audioSource.Play();
    }
    IEnumerator PlayWinMusic(){
        audioSource.clip = winMusic;
        audioSource.Stop();
        audioSource.Play();
        yield return new WaitForSeconds(winMusic.length);
        audioSource.Stop();
        audioSource.clip = gameOverMusicLoop;
        audioSource.Play();
    }
    void ManageWalletUI()
    {
        walletText.text = "Wallet: " + player.wallet + " / " + winMoney;
    }
    public void InstanceFirstRoom()
    {
        GameObject curRoom = Instantiate(demoRoom, transform);
        curRoom.transform.localPosition = new Vector3(0, 0, 0);
        Room rm = curRoom.GetComponent<Room>();
        rm.DeleteTriggers();
        rm.ShowDoors();
        rooms.Add(rm);

        curRoom = null;
        curRoom = Instantiate(roomPrefab, transform);
        curRoom.transform.localPosition = rooms[0].doorA.localPosition;
        rooms.Add(curRoom.GetComponent<Room>());

        curRoom = null;
        curRoom = Instantiate(roomPrefab, transform);
        curRoom.transform.localPosition = rooms[0].doorB.localPosition;
        rooms.Add(curRoom.GetComponent<Room>());

        curRoom = null;
        curRoom = Instantiate(roomPrefab, transform);
        curRoom.transform.localPosition = rooms[0].doorC.localPosition;
        rooms.Add(curRoom.GetComponent<Room>());
    }

    public void Update()
    {
        ManageWalletUI();
        if (gameStarted && !gameOver)
        {
            if(player.wallet >= winMoney){
                EndGame();
                StartCoroutine(PlayWinMusic());
            }
            
            if(player.wallet < loseMoney){
                destructionText.enabled = true;
                gameOver = true;
                StartCoroutine(PlayGameOverMusic());
            } else if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                timerUI.text = timeRemaining.ToString("f0");
            }
            else
            {
                EndGame();
                StartCoroutine(PlayGameOverMusic());
            }
        }
    }

    public void StartGame()
    {
        rooms[0].transform.GetChild(10).gameObject.SetActive(false);
        StartCoroutine(PlayMetalMusic());
        StartCoroutine(StartGameAfterSeconds());
    }

    public void EndGame()
    {
        gameOver = true;
        if (player.wallet >= winMoney)
        {
            winText.enabled = true;
        }
        else
        {
            loseText.enabled = true;
        }
    }
    IEnumerator StartGameAfterSeconds()
    {
        rooms[0].transform.GetChild(11).gameObject.SetActive(true);
        yield return new WaitForSeconds(countDown);
        rooms[0].transform.GetChild(11).gameObject.SetActive(false);
        rooms[0].transform.GetChild(8).gameObject.SetActive(false);
        gameStarted = true;
    }


    public void EnteredRoom(Room room, string direction)
    {
        for (int i = 0; i < rooms.Count - 4; i++)
        {
            if (rooms[i] != room)
            {
                Destroy(rooms[i].gameObject);
            }
        }
        rooms.RemoveAll(x => x != room);

        if (direction != "triggerA")
        {
            GameObject curRoom = Instantiate(roomPrefab, transform);
            curRoom.transform.position = room.doorA.position;
            rooms.Add(curRoom.GetComponent<Room>());
        }

        if (direction != "triggerB")
        {
            GameObject curRoom = Instantiate(roomPrefab, transform);
            curRoom.transform.position = room.doorB.position;
            rooms.Add(curRoom.GetComponent<Room>());
        }

        if (direction != "triggerC")
        {
            GameObject curRoom = Instantiate(roomPrefab, transform);
            curRoom.transform.position = room.doorC.position;
            rooms.Add(curRoom.GetComponent<Room>());
        }

        if (direction != "triggerD")
        {
            GameObject curRoom = Instantiate(roomPrefab, transform);
            curRoom.transform.position = room.doorD.position;
            rooms.Add(curRoom.GetComponent<Room>());
        }

    }

}
