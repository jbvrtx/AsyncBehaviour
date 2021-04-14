using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class AsyncBehaviour : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI label = null;

    private async void Start()
    {
        // Main thread
        string message = "";
        message += PrintThread("Main Thread Print") + "\n";

        // Task.Run / Task.ContinueWith
        Task<string> task1 = Task.Run(() => PrintThreadAsync("Task.Run()", 500)); // other thread
        Task<string> task2 = task1.ContinueWith(_ => PrintThread("Continue task1")); // other thread
        Task<string> task3 = task2.ContinueWith(_ => PrintThread("Continue task2 in current Context"), TaskScheduler.FromCurrentSynchronizationContext()); // main thread
        Task<string> task4 = task3.ContinueWith(_ => PrintThread("Continue task3")); // other thread

        // Await behaviour
        List<Task<string>> taskList = new List<Task<string>>(); // All on main thread
        taskList.Add(PrintThreadAsync("Task.WhenAll() - taskList 1", 1000));
        taskList.Add(PrintThreadAsync("Task.WhenAll() - taskList 2", 1000));
        taskList.Add(PrintThreadAsync("Task.WhenAll() - taskList 3", 1000));
        taskList.Add(PrintThreadAsync("Task.WhenAll() - taskList 4", 1000));
        taskList.Add(PrintThreadAsync("Task.WhenAll() - taskList 5", 1000));

        // Await results and print them on main thread
        await Task.WhenAll(taskList);
        message += task1.Result + "\n";
        message += task2.Result + "\n";
        message += task3.Result + "\n";
        message += task4.Result + "\n";
        taskList.ForEach(taskInList => message += taskInList.Result + "\n");

        label.text = message;
    }

    async Task<string> PrintThreadAsync(string functionName, int delay)
    {
        await Task.Delay(delay);

        return PrintThread(functionName);
    }

    // Get Thread data
    private string PrintThread(string functionName)
    {
        return "AsyncLog: " + functionName + ": " + Thread.CurrentThread.ManagedThreadId + ", T: " + System.Threading.Thread.CurrentThread.Name;
    }
}
