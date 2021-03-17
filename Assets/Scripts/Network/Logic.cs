using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

[RequireComponent(typeof(NetworkedObject))]
public class Logic : NetworkedBehaviour
{
    public GameObject player;

    float timer;

    int tickNumber;

    Queue<InputMessage> inputMessages = new Queue<InputMessage>();

    new Rigidbody rigidbody;

    private void Start()
    {
        timer = 0.0f;

        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(IsLocalPlayer)
        {
            timer += Time.deltaTime;
            while (timer >= Time.fixedDeltaTime)
            {
                timer -= Time.fixedDeltaTime;
                Inputs inputs = SampleInputs();

                InputMessage inputMessage;
                inputMessage.inputs = inputs;
                inputMessage.tickNumber = tickNumber;
                SendToServer(inputMessage);

                AddForcesToPlayer(rigidbody, inputs);

                Physics.Simulate(Time.fixedDeltaTime);

                ++tickNumber;
            }
        }
        else if(IsHost)
        {
            while(inputMessages.Count > 0)
            {
                InputMessage inputMessage = inputMessages.Dequeue();
                AddForcesToPlayer(rigidbody, inputMessage.inputs);

                Physics.Simulate(Time.fixedDeltaTime);

                StateMessage stateMessage;
                stateMessage.position = rigidbody.position;
                stateMessage.rotation = rigidbody.rotation;
                stateMessage.velocity = rigidbody.velocity;
                stateMessage.angularVelocity = rigidbody.angularVelocity;
                stateMessage.tickNumber = inputMessage.tickNumber;
            }
        }
    }

    Inputs SampleInputs()
    {
        Inputs inputs;
        inputs.up = Input.GetKey(KeyCode.W);
        inputs.down = Input.GetKey(KeyCode.S);
        inputs.left = Input.GetKey(KeyCode.A);
        inputs.right = Input.GetKey(KeyCode.D);
        inputs.jump = Input.GetKey(KeyCode.Space);
        return inputs;
    }

    void AddForcesToPlayer(Rigidbody rigidbody, Inputs inputs)
    {
        Vector3 force = Vector3.zero;
        if (inputs.right)
            force.x = 10f;
        if (inputs.left)
            force.x = -10f;
        if (inputs.up)
            force.z = 10f;
        if (inputs.down)
            force.z = -10f;
        if (inputs.jump)
            force.y = 20f;

        rigidbody.AddForce(force);
    }

    [ServerRPC(RequireOwnership = true)]
    void SendToServer(InputMessage inputMessage)
    {
        if(!IsHost)
            AddForcesToPlayer(rigidbody, inputMessage.inputs);
    }

    [ClientRPC]
    void SendToClient(StateMessage stateMessage)
    {
        
    }
}
