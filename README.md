# Top Down
This research assess the balance between characters in a top-down asymetric MOBA game using deep reinforcement learning.

# Setup

Clone the repositorys in a parent directory as follows

    git clone https://github.com/Taikatou/top-down-shooter.git
    git clone https://github.com/Taikatou/ml-agents.git

Follow relevant Python installation instructions for ml-agents [here](https://github.com/Taikatou/ml-agents/blob/master/docs/Installation.md)

Import the Unity TopDown Engine [here](https://assetstore.unity.com/packages/templates/systems/topdown-engine-89636)

# Training
After installation of ml-agents run the mlagents-learn command as shown bellow then press play in the editor.

    mlagents-learn ml-agents/config/config_ppo.yaml --train --run-id=<identifiable name>
