# Setup

Clone the repositorys in a parent directory as follows

    git clone https://github.com/Taikatou/top-down-shooter.git
    git clone https://github.com/Taikatou/ml-agents.git

Follow relevant Python installation instructions for ml-agents [here](https://github.com/Taikatou/ml-agents/blob/master/docs/Installation.md)

# Training
After installation of ml-agents run the mlagents-learn command as shown bellow then press play in the editor.

    mlagents-learn ml-agents/config/config_ppo.yaml --train --run-id=<identifiable name>
