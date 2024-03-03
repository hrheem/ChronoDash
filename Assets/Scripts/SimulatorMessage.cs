using System;

public class SimulatorMessage
{

    public float timeVal = 0;
    public float timeDelta = 0;

    public float xPos = 0;
    public float yPos = 0;
    public float zPos = 0;

    public float speedVal = 0;
    public float headingVal = 0;

    public float steerVal = 0;
    public float gasPedalVal = 0;
    public float brakePedalVal = 0;

    public float state1 = 0;
    public float state2 = 0;
    public float state3 = 0;
    public float state4 = 0;
    public float state5 = 0;
    public float state6 = 0;

    public float eulerX = 0;
    public float eulerY = 0;
    public float eulerZ = 0;

    public float rpm = 0;
    public float gear = 0;

    public float indicator1 = 0;
    public float indicator2 = 0;

    // Start is called before the first frame update
    public SimulatorMessage(byte[] bData, bool isChrono = true)
    {

        if (isChrono)
        {

            speedVal = (float)BitConverter.ToSingle(bData, 2);
            rpm = (float)BitConverter.ToSingle(bData, 3);

        }
        else
        {

            timeVal = BitConverter.ToSingle(bData, 0);
            timeDelta = BitConverter.ToSingle(bData, 4);

            xPos = BitConverter.ToSingle(bData, 8);
            yPos = BitConverter.ToSingle(bData, 12);
            zPos = BitConverter.ToSingle(bData, 16);

            speedVal = BitConverter.ToSingle(bData, 20);
            headingVal = BitConverter.ToSingle(bData, 24);

            steerVal = BitConverter.ToSingle(bData, 28);
            gasPedalVal = BitConverter.ToSingle(bData, 32);
            brakePedalVal = BitConverter.ToSingle(bData, 36);

            state1 = BitConverter.ToSingle(bData, 40);
            state2 = BitConverter.ToSingle(bData, 44);
            state3 = BitConverter.ToSingle(bData, 48);
            state4 = BitConverter.ToSingle(bData, 52);
            state5 = BitConverter.ToSingle(bData, 56);
            state6 = BitConverter.ToSingle(bData, 60);

            eulerX = BitConverter.ToSingle(bData, 64);
            eulerY = BitConverter.ToSingle(bData, 68);
            eulerZ = BitConverter.ToSingle(bData, 72);

            rpm = BitConverter.ToSingle(bData, 76);
            gear = BitConverter.ToSingle(bData, 80);

            indicator1 = BitConverter.ToSingle(bData, 84);
            indicator2 = BitConverter.ToSingle(bData, 88);

        }

    }

}
