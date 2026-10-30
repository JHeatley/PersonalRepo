@echo off
rem Starts the GPU learner for the Neural AI's training (keep this window open while training).
rem The first time, it sets up its own Python packages (PyTorch for NVIDIA cards, about 3 GB) in the .venv folder here.
rem Needs Python 3.10 or newer from python.org (tick "Add python.exe to PATH") and an NVIDIA graphics card.
rem Without it, training still works: it learns on the CPU instead.
cd /d "%~dp0"
where python >nul 2>nul
if errorlevel 1 (
  echo Python isn't installed. Get Python 3.10 or newer from python.org, tick "Add python.exe to PATH", then run this again.
  pause
  exit /b 1
)
if not exist ".venv\ready.txt" (
  echo Setting up the GPU learner the first time: PyTorch for NVIDIA cards, about 3 GB. This takes a while.
  if not exist ".venv\Scripts\python.exe" python -m venv .venv
  if errorlevel 1 goto failed
  ".venv\Scripts\python.exe" -m pip install --upgrade pip
  ".venv\Scripts\python.exe" -m pip install numpy
  if errorlevel 1 goto failed
  ".venv\Scripts\python.exe" -m pip install torch --index-url https://download.pytorch.org/whl/cu128
  if errorlevel 1 ".venv\Scripts\python.exe" -m pip install torch --index-url https://download.pytorch.org/whl/cu126
  if errorlevel 1 goto failed
  echo ready> ".venv\ready.txt"
)
".venv\Scripts\python.exe" -c "import torch, sys; sys.exit(0 if torch.cuda.is_available() else 1)"
if errorlevel 1 (
  echo PyTorch can't see an NVIDIA graphics card. Updating the NVIDIA driver usually fixes it.
  echo Training still works without the GPU learner: it learns on the CPU.
  pause
  exit /b 1
)
".venv\Scripts\python.exe" learner.py %*
pause
exit /b 0

:failed
echo Setting up the GPU learner failed (see above). Check your internet connection and run this again.
pause
exit /b 1
