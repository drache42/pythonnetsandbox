# sfroehlich-pythonnet


## Setup to run python Code
1. `cd` into `src\PythonProgram`
2. Create a virtual env
    - `python3 -m <venv name>`
3. Activate your environment
    - Run the activate script in the <venv/Scripts/> directory
4. Install the requirements
    - pip install -r requirements.txt

## To run python script
Look up how to do this in your favorite IDE, or follow these instructions

### Prereqs
Build the dotnet binaries
1. `dotnet publish -o dist`
2. Activate your environment
    - Run the activate script in the <venv/Scripts/> directory
3. `python PythonProgram.py`