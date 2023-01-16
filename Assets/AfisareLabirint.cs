using OpenCover.Framework.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfisareLabirint : MonoBehaviour
{
    #region Variabile

    public enum tipAlgoritm // your custom enumeration
    {
        DFS,
        BFS
    };
    public enum pointOfView
    {
        FirstPerson=0,
        ThirdPerson=1,
        Spectator=2
    };

    public GameObject obiect;
    public GameObject cubSolutie;
    public GameObject camera;
    public File MatriceLabirint;
    public int startX = 1;
    public int startY = 1;
    public tipAlgoritm Tip_Parcurgere = tipAlgoritm.DFS;
    public pointOfView Camera = pointOfView.Spectator;
    public float delay = (float)0.2;

    Stack<Node> stackDeDesenat = new Stack<Node>();
    int cameraPOV;
    bool stop = false;
    Vector3 pozitie; // pozitia a zonei
    System.Random r = new System.Random();
    static int[,] labirint = citireLabirint();
    int[,] path = new int[labirint.GetLength(0), labirint.GetLength(1)];
    #endregion

    public class Node
    {
        public int x, y;
        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    int dfAlg(int i, int j)
    {
        if (i < 0 || i >= labirint.GetLength(0) || j < 0 || j >= labirint.GetLength(1))
        {
            Debug.Log("Succes");
            return 2;
        }
        if (labirint[i, j] == 1 || path[i, j] == 1 || path[i, j] == 2)
        {
            return 1;
        }
        if (labirint[i, j] == 0)
        {
            int[] directii = new int[4];
            path[i, j] = 1;//E ok, putem merge pe acolo
            directii[0] = dfAlg(i, j + 1);
            directii[1] = dfAlg(i + 1, j);
            directii[2] = dfAlg(i, j - 1);
            directii[3] = dfAlg(i - 1, j);
            foreach (int dir in directii)
            {
                if (dir == 2)
                {
                    Node nod = new Node(i, j);
                    stackDeDesenat.Push(nod);
                    path[i, j] = dir;
                    break;
                }
            }
        }
        return path[i, j];
    }


    void generatePathBFS(int i, int j)
    {
        if (stop) return;
        if (i < 0 || i >= labirint.GetLength(0) || j < 0 || j >= labirint.GetLength(1))
        {
            stop = true;
            return;
        }
        if (labirint[i, j] == 1 || path[i, j] == 1)
        {
            return;
        }
        if (labirint[i, j] == 0)
        {
            path[i, j] = 1;
            Node helper = new Node(i, j);
            stackDeDesenat.Push(helper);
            generatePathBFS(i - 1, j);
            generatePathBFS(i + 1, j);
            generatePathBFS(i, j - 1);
            generatePathBFS(i, j + 1);
        }

    }

    public bool IsValidMove(int x, int y)
    {
        if (x < 0 || x >= labirint.GetLength(0) || y < 0 || y >= labirint.GetLength(1))
        {
            return false;
        }
        else if (labirint[x, y] == 1 || path[x, y] == 1 || path[x, y] == 2)
        {
            return false;
        }

        return true;
    }

    void Start()
    {
        cameraPOV= (int)Camera;
        labirintDeCuburi();
        Vector3 offset = new Vector3(startX + pozitie.x, 0, startY + pozitie.z);
        Instantiate(cubSolutie, offset, Quaternion.identity);

        if (Tip_Parcurgere==tipAlgoritm.DFS) //====== DFS ======
        {
            path[startX, startY] = dfAlg(startX, startY);
        }
        else if (Tip_Parcurgere==tipAlgoritm.BFS) //====== BFS ======
        {
            generatePathBFS(1, 1);
            stackDeDesenat = ReverseStack(stackDeDesenat);
        }

        StartCoroutine(deseneazaPath());
    }

    private IEnumerator deseneazaPath()
    {
        float oldX = 1;
        float oldY = 1;
        float oldoldX = 1;
        float oldoldY = 1;
        foreach (Node nod in stackDeDesenat)
        {

            if (cameraPOV == 0)
            {
                if (oldX > nod.x)
                {
                    camera.transform.LookAt(new Vector3(-100, 0, oldY));
                    oldX = nod.x;
                }
                if (oldX < nod.x)
                {
                    camera.transform.LookAt(new Vector3(100, 0, oldY));
                    oldX = nod.x;
                }
                if (oldY > nod.y)
                {
                    camera.transform.LookAt(new Vector3(oldX, 0, -100));
                    oldY = nod.y;
                }
                if (oldY < nod.y)
                {
                    camera.transform.LookAt(new Vector3(oldX, 0, 100));
                    oldY = nod.y;
                }

                Vector3 offset = new Vector3(nod.x, 0, nod.y);
                camera.transform.position = offset;
            }
            if (cameraPOV == 1)
            {

                if (oldX > nod.x)
                {
                    camera.transform.LookAt(new Vector3(-100, -25f, oldY));
                    oldX = nod.x;
                }
                if (oldX < nod.x)
                {
                    camera.transform.LookAt(new Vector3(100, -25f, oldY));
                    oldX = nod.x;
                }
                if (oldY > nod.y)
                {
                    camera.transform.LookAt(new Vector3(oldX, -25f, -100));
                    oldY = nod.y;
                }
                if (oldY < nod.y)
                {
                    camera.transform.LookAt(new Vector3(oldX, -25f, 100));
                    oldY = nod.y;
                }

                Vector3 offset = new Vector3(oldoldX, 1.5f, oldoldY);
                camera.transform.position = offset;
            }
            if (cameraPOV == 2)
            {
                float width = labirint.GetLength(0);
                float length = labirint.GetLength(1);
                float angleRad = 30 * Mathf.Deg2Rad;
                float height;
                if (width >= length)
                {
                    height = width * Mathf.Tan(angleRad);
                }
                else
                {
                    height = length * Mathf.Tan(angleRad);
                }
                camera.transform.LookAt(new Vector3(width / 2, 0, length / 2));
                camera.transform.Rotate(0, 0, 90);
                Vector3 offset = new Vector3(width / 2, height, length / 2);
                camera.transform.position = offset;
            }
            oldoldX = oldX;
            oldoldY = oldY;

            Vector3 offsetCub = new Vector3(nod.x + pozitie.x, 0, nod.y + pozitie.z);
            Instantiate(cubSolutie, offsetCub, Quaternion.identity);
            yield return new WaitForSeconds((float)delay);
        }
    }

    static int[,] citireLabirint()
    {
        string[] lines = System.IO.File.ReadAllLines(@"E:\De pe pc vechi\facultate\Facultate\game dev\Labirint\LabirintUnity\Labirint.txt");
        int[,] lab = new int[lines.Length, lines[0].Length];
        int i = 0, j = 0;
        foreach (string line in lines)
        {
            j = 0;
            foreach (char c in line)
            {
                lab[i, j] = (int)Char.GetNumericValue(c);
                j++;
            }
            i++;
        }
        return lab;
    }
    void labirintDeCuburi()
    {

        for (int i = 0; i < labirint.GetLength(0); i++)
        {
            for (int j = 0; j < labirint.GetLength(1); j++)
            {
                if (labirint[i, j] == 1)
                {
                    Vector3 offset = new Vector3(i + pozitie.x, 0, j + pozitie.z);
                    Instantiate(obiect, offset, Quaternion.identity);
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            cameraPOV = (cameraPOV + 1) % 3;
        }
    }
    public Stack<T> ReverseStack<T>(Stack<T> stack)
    {
        var reversedStack = new Stack<T>();
        while (stack.Count > 0)
        {
            reversedStack.Push(stack.Pop());
        }
        return reversedStack;
    }
}