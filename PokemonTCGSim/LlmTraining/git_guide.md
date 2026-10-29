# Git, GitHub and GitHub Desktop: questions and answers

The readable version of `extra/git.jsonl` (the same content, which `build_dataset.py` adds to the training data). The Git commands were run and checked (Git 2.43).

# Git

## What is Git, and how is it different from GitHub?

**Git** is a version control system: a program on your computer that records snapshots (commits) of a project's files over time, so you can see what changed, go back to any earlier version, and work on several lines of development (branches) at once. It works entirely offline; every copy of a repository has its full history.

**GitHub** is a website that hosts Git repositories online. It adds what Git itself doesn't have: a place to share and back up your repositories, pull requests and code review, issues, GitHub Actions (automated builds and tests), releases, wikis and access control.

**GitHub Desktop** is a free app from GitHub that gives Git a graphical interface: you commit, branch, push and pull with buttons instead of commands. It still uses Git underneath, so everything it does can also be done on the command line, and the two can be mixed freely on the same repository.

Other Git hosts work the same way: GitLab, Bitbucket, Azure DevOps.

## What are the working directory, the staging area and a commit in Git?

Git has three places a change can be:

1. **Working directory (working tree)**: the files as they are on disk now, including edits you haven't saved into Git.
2. **Staging area (the index)**: the changes you've chosen to include in the next commit (`git add` puts them there).
3. **Repository (commits)**: snapshots saved permanently in the `.git` folder (`git commit` turns what's staged into a commit).

```
edit files  →  git add <files>  →  git commit -m "message"
(working)       (staged)            (committed)
```

The staging area lets you commit only part of your work, for example a bug fix but not the half-finished feature in another file. `git status` shows which files are in which state.

A **commit** is a snapshot of the whole project plus metadata (author, date, message) and a link to its parent commit(s). Each commit has a unique ID, a 40-character SHA hash such as `3409072a...`; the first 7 characters are usually enough to refer to it.

## How do I install and set up Git on Windows?

1. Download **Git for Windows** from git-scm.com and install it. The defaults are fine; it also installs **Git Bash** and **Git Credential Manager** (which handles GitHub sign-in for you).
2. Open a new terminal (PowerShell, Command Prompt or Git Bash) and set who you are. This name and email go into every commit you make:

```
git config --global user.name "Your Name"
git config --global user.email "you@example.com"
git config --global init.defaultBranch main
git config --global core.autocrlf true      # Windows: check out with CRLF, commit with LF
git config --global core.editor "code --wait"   # optional: use VS Code for commit messages
git --version
git config --global --list
```

Use the email of your GitHub account (or GitHub's private `noreply` address from Settings → Emails) so commits are linked to your profile.

GitHub Desktop includes its own copy of Git, so you can use Desktop without installing Git separately, but installing Git for Windows also gives you the command line.

## How do I create a new Git repository or copy an existing one?

**Start tracking an existing folder:**

```
cd C:\Projects\MyGame
git init                      # creates the hidden .git folder: this is now a repository
git add .
git commit -m "First commit"
```

**Copy (clone) a repository from GitHub:**

```
git clone https://github.com/owner/repo.git
git clone https://github.com/owner/repo.git MyFolder   # into a folder with another name
cd repo
```

Cloning downloads the full history and sets up a **remote** called `origin` pointing back to GitHub, so `git pull` and `git push` know where to go.

Don't run `git init` inside a folder that's already inside another repository (it creates a nested repository, which confuses things). Deleting the `.git` folder removes all history and turns the folder back into ordinary files.

## What do git status, git add and git commit do?

```
git status                     # what changed, what's staged, which branch you're on
git add Assets/Scripts/Player.cs   # stage one file
git add Assets/                # stage everything under a folder
git add .                      # stage all changes in the current folder and below (new, changed, deleted)
git add -p                     # go through changes piece by piece and choose which to stage
git commit -m "Player jumps higher"                  # commit what's staged
git commit -m "Short summary" -m "Longer explanation of why."
git commit -am "Message"       # stage all changes to already-tracked files, then commit (skips new files)
```

`git status -s` gives a short version: `M` modified, `A` added, `D` deleted, `??` untracked (new and not yet added); the left column is the staging area, the right the working tree.

Good commit messages: a short summary line (about 50 characters, in the imperative: "Fix jump height", not "Fixed"), then a blank line and the details if needed. Commit small, related changes together; it makes history easy to read and changes easy to undo.

## How do I see the history and what changed in Git?

```
git log                         # full history, newest first (q to quit)
git log --oneline               # one line per commit
git log --oneline --graph --all # with branches drawn as a graph
git log -5                      # the last 5
git log -- Assets/Scripts/Player.cs   # commits that changed one file
git log --author="Jane" --since="2 weeks ago"
git log -S "JumpHeight"         # commits that added or removed this text

git diff                        # unstaged changes (working tree vs staging area)
git diff --staged               # what's staged for the next commit
git diff main..feature          # between two branches
git diff HEAD~3 -- Player.cs    # one file, compared with three commits ago

git show 3409072                # one commit: message and changes
git show HEAD:Assets/Scripts/Player.cs   # a file as it was in a commit
git blame Assets/Scripts/Player.cs       # who last changed each line, and in which commit
```

`HEAD` means "the commit you're on now"; `HEAD~1` is its parent, `HEAD~3` three commits back.

## What should a .gitignore file contain for a Unity project?

A `.gitignore` file in the repository's root lists files Git should not track. For Unity, ignore everything Unity regenerates, and keep `Assets/`, `Packages/` and `ProjectSettings/`:

```
# Unity generated folders
/[Ll]ibrary/
/[Tt]emp/
/[Oo]bj/
/[Bb]uild/
/[Bb]uilds/
/[Ll]ogs/
/[Uu]ser[Ss]ettings/
/[Mm]emoryCaptures/
/[Rr]ecordings/

# IDE files Unity regenerates
.vs/
.idea/
*.csproj
*.sln
*.suo
*.user
*.pidb
*.booproj

# OS files
.DS_Store
Thumbs.db

# Builds and crash files
*.apk
*.aab
*.unitypackage
sysinfo.txt
crashlytics-build.properties
```

GitHub's official template (github.com/github/gitignore, `Unity.gitignore`) is a good start, and you can pick "Unity" when creating a repository on GitHub or in GitHub Desktop.

Rules: `folder/` ignores a folder, `*.log` a pattern, a leading `/` anchors to the repository root, and `!important.log` makes an exception. **Always commit the `.meta` files** next to your assets: Unity stores each asset's ID and import settings there, and losing them breaks references in scenes and prefabs.

## I added a file to .gitignore but Git still tracks it. Why?

`.gitignore` only affects files that aren't tracked yet. A file that was already committed stays tracked until you tell Git to stop tracking it:

```
git rm --cached secrets.json          # stop tracking the file, keep it on disk
git rm -r --cached Library/           # a whole folder
git commit -m "Stop tracking generated files"
```

After that commit, `.gitignore` keeps it out. Other people get the file deleted from their working copy when they pull that commit, so warn them if it's something they need locally (like a settings file).

If the file contained secrets (passwords, API keys), removing it now isn't enough: it's still in the history. Change the secret (revoke the key) first, then, if needed, rewrite history with `git filter-repo` and force-push.

## How do I undo changes in Git?

It depends on where the change is:

**Not staged yet (just edited):**
```
git restore Player.cs          # throw away the edits to one file (back to the last commit)
git restore .                  # throw away all unstaged edits (careful: can't be undone)
```

**Staged, but not committed:**
```
git restore --staged Player.cs   # unstage (the edits stay in the file)
```

**Committed, but not pushed:**
```
git commit --amend -m "Better message"   # fix the last commit's message (or add forgotten staged files)
git reset --soft HEAD~1    # undo the last commit, keep its changes staged
git reset HEAD~1           # undo the last commit, keep its changes as unstaged edits (--mixed, the default)
git reset --hard HEAD~1    # undo the last commit and throw its changes away (careful)
```

**Already pushed (others may have it):**
```
git revert 3409072         # a new commit that undoes that commit; history isn't rewritten
```

Rule of thumb: rewrite history (`reset`, `--amend`, `rebase`) only for commits nobody else has; for pushed commits, use `revert`. Older guides use `git checkout -- file` instead of `git restore`; it does the same thing.

## What's the difference between git reset --soft, --mixed and --hard?

All three move the current branch to another commit (for example `HEAD~1`, one back). They differ in what happens to your files:

| Command | Branch moves | Staging area | Working files |
|---|---|---|---|
| `git reset --soft HEAD~1` | yes | keeps the undone commit's changes, staged | untouched |
| `git reset HEAD~1` (`--mixed`) | yes | cleared | keeps the changes as edits |
| `git reset --hard HEAD~1` | yes | cleared | **changes thrown away** |

```
# Squash the last 3 commits into one:
git reset --soft HEAD~3
git commit -m "One commit for the whole feature"
```

`--hard` is the only one that loses work, and only uncommitted work is truly lost: committed work can still be found with `git reflog` for a while. If the commits were pushed already, resetting means you'll need a force push, which rewrites the shared history; use `git revert` instead on shared branches.

## I lost commits after a reset or a bad rebase. Can I get them back?

Usually yes. Git keeps a log of every position `HEAD` has been at, the **reflog**, for about 90 days:

```
git reflog
# a1b2c3d HEAD@{0}: reset: moving to HEAD~2
# 9f8e7d6 HEAD@{1}: commit: Add double jump      <- the commit you lost
# ...
git reset --hard 9f8e7d6        # put the branch back there
# or keep the current state and just get the commit back on a new branch:
git branch rescue 9f8e7d6
```

A deleted branch can be recovered the same way: find its last commit in `git reflog` and run `git branch name <commit>`.

What can't be recovered: changes that were never committed or stashed (for example edits discarded with `git restore` or `git reset --hard`). Commit or stash often; it's what makes Git safe.

## How does git stash work?

Stash shelves your uncommitted changes so you get a clean working tree, and lets you bring them back later, for example to switch branches or pull without committing half-finished work:

```
git stash                      # shelve changes to tracked files
git stash -u                   # include untracked (new) files too
git stash push -m "jump tweak" Player.cs   # stash only some files, with a name
git stash list                 # stash@{0}: On main: jump tweak
git stash pop                  # re-apply the latest stash and remove it from the list
git stash apply stash@{1}      # re-apply a specific one and keep it in the list
git stash drop stash@{1}       # delete one
git stash show -p stash@{0}    # see what's in it
git stash branch new-feature   # make a branch from a stash
```

If `pop` hits a conflict, the stash isn't removed: fix the conflict, then `git stash drop`. GitHub Desktop has the same feature: Branch → Stash all changes, and the stash shows at the bottom of the Changes list to restore or discard.

## What do git clean and git restore do, and how do I reset everything to the last commit?

- `git restore <file>` puts a tracked file back as it was in the last commit (discarding edits).
- `git clean` deletes **untracked** files (new files Git doesn't know about). Always preview first:

```
git clean -n        # dry run: list what would be deleted
git clean -f        # delete untracked files
git clean -fd       # ... and untracked folders
git clean -fdx      # ... and ignored files too (e.g. Unity's Library/: forces a full reimport)
```

To make your working copy exactly match the last commit:

```
git reset --hard HEAD
git clean -fd
```

Both throw away uncommitted work for good; stash it (`git stash -u`) if you might want it back.

## What is a branch in Git and how do I use branches?

A branch is a movable name pointing at a commit. When you commit on a branch, the name moves forward to the new commit. Branches let you work on something (a feature, a bug fix, an experiment) without touching the main line until it's ready.

```
git branch                     # list branches (* marks the current one)
git branch -a                  # include remote branches
git switch -c new-enemy-ai     # create a branch and switch to it
git switch main                # switch back
git branch -d new-enemy-ai     # delete a branch that's been merged
git branch -D experiment       # delete one that hasn't (its unmerged commits become unreachable)
git branch -m old-name new-name   # rename
```

Older commands do the same: `git checkout -b name` creates and switches, `git checkout name` switches.

Switching branches changes the files on disk to that branch's version. Commit or stash your changes first; Git refuses to switch if it would overwrite uncommitted edits.

A common workflow: `main` always works; every change is made on its own branch, merged into `main` when done (often through a pull request on GitHub), and the branch is deleted.

## How do I merge branches in Git?

Switch to the branch you want to bring changes **into**, then merge the other one:

```
git switch main
git merge new-enemy-ai
```

Two things can happen:

- **Fast-forward**: if `main` hasn't changed since the branch was made, Git just moves `main` forward to the branch's last commit. No merge commit.
- **Merge commit**: if both branches have new commits, Git combines them and creates a merge commit with two parents.

```
git merge --no-ff feature      # always make a merge commit (keeps the branch visible in history)
git merge --squash feature     # combine the branch's changes into one staged change, then commit it yourself
git merge --abort              # give up a merge that hit conflicts
```

If both branches changed the same lines, the merge stops with a **conflict** for you to resolve. After merging, delete the branch: `git branch -d new-enemy-ai`.

## How do I resolve a merge conflict in Git?

A conflict happens when both sides changed the same part of a file. Git stops and marks the spot in the file:

```
<<<<<<< HEAD
    public float jumpHeight = 2.0f;
=======
    public float jumpHeight = 3.5f;
>>>>>>> new-movement
```

The part above `=======` is your current branch (`HEAD`), below it is the branch being merged.

1. `git status` lists the conflicted files ("both modified").
2. Open each one, decide what the code should be (one side, the other, or a mix), and delete the `<<<<<<<`, `=======` and `>>>>>>>` lines. VS Code shows buttons: "Accept Current", "Accept Incoming", "Accept Both".
3. Stage the fixed files and finish:

```
git add Assets/Scripts/Player.cs
git commit          # (or git merge --continue / git rebase --continue during a rebase)
```

To take one side for a whole file: `git checkout --ours file` (current branch) or `git checkout --theirs file` (the other), then `git add` it. Changed your mind: `git merge --abort` puts everything back as before the merge.

For Unity scenes and prefabs, conflicts are hard to fix by hand: use Unity's Smart Merge tool (UnityYAMLMerge), or avoid two people editing the same scene at once.

## What is git rebase, and when should I use it instead of merge?

`git rebase main` (on a feature branch) takes your branch's commits and replays them on top of the latest `main`, as if you'd started the branch from there. The result is a straight line of history instead of a merge commit:

```
git switch feature
git rebase main          # replay feature's commits on top of main
# conflicts: fix the file, git add it, then
git rebase --continue    # or git rebase --abort to go back
```

**Interactive rebase** tidies your own commits before sharing them:

```
git rebase -i HEAD~4     # edit the last 4 commits: reorder, squash, reword or drop them
```

In the editor that opens, change `pick` to `squash` (or `s`) to fold a commit into the one above it, `reword` to change its message, `drop` to remove it.

**The golden rule: don't rebase commits that others already have** (pushed to a shared branch). Rebasing creates new commits with new IDs, so everyone else's copies no longer match. Use it on your own local branch; use merge (or GitHub's pull requests) for shared ones. After rebasing a branch you'd already pushed, only you use, you need `git push --force-with-lease`.

## What does git cherry-pick do?

It copies one commit from another branch onto the current branch, as a new commit:

```
git switch release
git cherry-pick 3409072          # bring just that fix over
git cherry-pick a1b2c3d..f6e5d4c # a range (the first one is excluded)
git cherry-pick -n 3409072       # apply the changes without committing (to edit first)
git cherry-pick --abort          # if it conflicts and you want to stop
```

Typical use: a bug fix made on a feature branch is needed on `main` now, without merging the whole unfinished feature. If you later merge the feature branch too, Git usually notices the change is already there. GitHub Desktop does the same by dragging a commit from the History list onto another branch.

## What is a detached HEAD in Git?

Normally `HEAD` points at a branch, and the branch points at a commit. **Detached HEAD** means `HEAD` points straight at a commit, not at a branch: it happens when you check out a commit ID, a tag or a remote branch directly (`git checkout 3409072`, `git checkout v1.0`).

You can look around, build and test the old version safely. If you make commits there, they don't belong to any branch, and they'll be lost (except through `git reflog`) when you switch away. To keep them, make a branch before switching:

```
git switch -c fix-from-old-version     # keeps the commits you made in detached HEAD
git switch main                        # or just go back if you were only looking
```

`git switch --detach v1.0` detaches on purpose; `git switch` refuses to detach by accident, which is one reason it's friendlier than the old `git checkout`.

## How do tags work in Git?

A tag is a permanent name for a commit, usually a release:

```
git tag v1.0                          # lightweight tag on the current commit
git tag -a v1.0 -m "First release"    # annotated tag (with author, date and message: use these for releases)
git tag -a v0.9 3409072 -m "Beta"     # tag an older commit
git tag                               # list tags
git show v1.0
git push origin v1.0                  # tags aren't pushed automatically
git push origin --tags                # push all tags
git tag -d v1.0                       # delete locally
git push origin --delete v1.0         # delete on GitHub
```

On GitHub, a tag can become a **Release** (Releases → Draft a new release), where you attach build files (a game's .zip or .exe) and write release notes.

## How do I find which commit introduced a bug with git bisect?

`git bisect` does a binary search through history:

```
git bisect start
git bisect bad                 # the current version has the bug
git bisect good v1.2           # this older version (tag or commit ID) didn't
# Git checks out a commit halfway between. Test it, then say:
git bisect good                # or: git bisect bad
# ... repeat; after a few steps Git prints "<commit> is the first bad commit"
git bisect reset               # back to where you started
```

With 1,000 commits between good and bad, it takes about 10 tests. If you have a script that exits with 0 when things work and 1 when they don't, Git can run it for you: `git bisect run python test_jump.py`.

# Remotes, large files, Unity and troubleshooting

## What are remotes, and what do git fetch, git pull and git push do?

A **remote** is another copy of the repository that yours syncs with, usually on GitHub. The one you cloned from is called `origin`.

```
git remote -v                      # list remotes and their URLs
git fetch                          # download new commits from origin; doesn't change your files or branches
git pull                           # fetch, then merge origin's version of your branch into yours
git push                           # upload your branch's new commits to origin
```

- **fetch** is always safe: it updates `origin/main` (your copy of what's on GitHub) and lets you look before merging (`git log main..origin/main`).
- **pull** = fetch + merge (or fetch + rebase with `git pull --rebase`, which keeps history straight). It can produce conflicts if both sides changed the same lines.
- **push** fails if GitHub has commits you don't have; pull first, then push again.

Tip: set `git config --global pull.rebase false` (always merge) or `true` (always rebase) once, so Git doesn't ask which you want.

## How do I push a new local branch to GitHub?

```
git switch -c new-menu
# ... commits ...
git push -u origin new-menu
```

`-u` (`--set-upstream`) links your local branch to `origin/new-menu`, so from then on plain `git push` and `git pull` work on that branch. Without it, Git says "The current branch has no upstream branch" and shows the command to run.

To make that automatic for every new branch: `git config --global push.autoSetupRemote true` (Git 2.37 and later).

After pushing, GitHub prints a link to open a pull request for the branch.

## How do I connect an existing local project to a new GitHub repository?

1. On GitHub: **New repository**, give it a name, and **don't** add a README, .gitignore or license (an empty repository avoids a first-pull conflict).
2. In your project folder:

```
git init                      # if it isn't a repository yet
git add .
git commit -m "First commit"
git branch -M main            # make sure the branch is called main
git remote add origin https://github.com/you/MyGame.git
git push -u origin main
```

If GitHub already has commits (you did add a README), pull them first: `git pull origin main --allow-unrelated-histories`, resolve anything that conflicts, then push.

With GitHub Desktop it's one step: File → Add local repository (or create one), then **Publish repository**, which creates the GitHub repository and pushes.

## Git says my push was rejected (non-fast-forward / fetch first). What do I do?

```
! [rejected]        main -> main (fetch first)
error: failed to push some refs ... Updates were rejected because the remote contains work that you do not have locally.
```

Someone (or you, from another computer, or a bot) pushed commits to the branch that you don't have yet. Git won't overwrite them. Bring them in, then push:

```
git pull            # merge their commits with yours (resolve conflicts if any)
git push
```

or `git pull --rebase` to put your commits on top of theirs for a straight history.

**Don't** "fix" this with `git push --force` on a shared branch: it deletes the other commits from GitHub. Force pushing is only for your own branch after rewriting its history, and then use `git push --force-with-lease`, which refuses if the remote has commits you haven't seen.

## What's the difference between git push --force and --force-with-lease?

Both replace the remote branch with your local one, even if that throws away commits on the remote. You need one after rewriting history you'd already pushed (`rebase`, `commit --amend`, `reset`).

- `--force` overwrites whatever is there, including commits someone else pushed a minute ago.
- `--force-with-lease` only overwrites if the remote branch is still where you last fetched it. If someone pushed in the meantime, it refuses, so you don't silently delete their work.

```
git push --force-with-lease
```

Always prefer `--force-with-lease`, and only on branches that are yours. Many teams protect `main` on GitHub so nobody can force-push to it at all.

## How do I update my branch with the latest changes from main?

```
git switch main
git pull                       # get the latest main
git switch my-feature
git merge main                 # bring main's new commits into the branch
```

or, if the branch is only yours and you prefer straight history:

```
git switch my-feature
git fetch
git rebase origin/main         # replay your commits on top of the latest main
git push --force-with-lease    # if you had pushed the branch before
```

Do this regularly on long-running branches: small conflicts now are much easier than one huge conflict at the end. On GitHub, a pull request whose branch is behind shows an **Update branch** button that does the merge for you.

## How do I work with a fork on GitHub?

A **fork** is your own copy of someone else's repository on GitHub, used to propose changes to projects you can't push to.

1. On the project's GitHub page: **Fork**.
2. Clone your fork, and add the original as a second remote called `upstream`:

```
git clone https://github.com/you/their-project.git
cd their-project
git remote add upstream https://github.com/them/their-project.git
```

3. Work on a branch, push it to your fork (`git push -u origin my-fix`), and open a pull request from your fork's branch to their `main`.
4. Keep your fork up to date:

```
git fetch upstream
git switch main
git merge upstream/main
git push
```

(GitHub also has a **Sync fork** button on your fork's page.)

## How do I use SSH keys instead of passwords for GitHub?

GitHub doesn't accept your account password for Git over HTTPS. Use either Git Credential Manager (installed with Git for Windows: it opens a browser sign-in the first time you push) or SSH keys:

```
ssh-keygen -t ed25519 -C "you@example.com"     # press Enter to accept the default file; a passphrase is optional
cat ~/.ssh/id_ed25519.pub                       # the PUBLIC key: copy this
```

On GitHub: Settings → SSH and GPG keys → New SSH key, paste it. Then:

```
ssh -T git@github.com                           # "Hi you! You've successfully authenticated..."
git clone git@github.com:you/MyGame.git
git remote set-url origin git@github.com:you/MyGame.git   # switch an existing clone to SSH
```

Never share the private key (`id_ed25519`, without `.pub`). For scripts and tools over HTTPS, use a **personal access token** (Settings → Developer settings) instead of a password, with only the permissions needed.

## What is Git LFS and do I need it for a Unity project?

**Git LFS (Large File Storage)** keeps big binary files (textures, audio, 3D models, videos) outside the normal Git history: the repository stores a small pointer, and the real file is stored on the LFS server and downloaded when needed. Without it, every version of every big file stays in the history forever, and the repository grows huge and slow.

GitHub refuses single files over **100 MB** (and warns from 50 MB) without LFS. For Unity projects with art and audio, LFS is strongly recommended:

```
git lfs install                                # once per computer
git lfs track "*.psd" "*.png" "*.fbx" "*.wav" "*.mp3" "*.mp4" "*.tga" "*.exr"
git add .gitattributes                         # the tracking rules live here: commit it
git add . && git commit -m "Track art and audio with LFS"
git lfs ls-files                               # what's stored in LFS
```

Set it up before committing the big files; files already in history stay there unless you migrate them (`git lfs migrate import --include="*.psd"`, which rewrites history).

GitHub's free LFS storage and bandwidth are limited (the amount is listed in your account's billing settings); large projects buy data packs. GitHub Desktop detects LFS files and handles them automatically.

## How should I set up Unity for working with Git?

1. **Edit → Project Settings → Editor**:
   - Version Control → Mode: **Visible Meta Files**
   - Asset Serialization → Mode: **Force Text** (scenes and prefabs saved as text, so Git can diff and merge them)
2. Add a Unity `.gitignore` (ignore `Library/`, `Temp/`, `Obj/`, `Build/`, `Logs/`, `UserSettings/`, generated `.csproj`/`.sln`).
3. **Commit the `.meta` files** with their assets, always. Each holds the asset's GUID; a missing or regenerated `.meta` breaks every reference to that asset in scenes and prefabs.
4. Use **Git LFS** for textures, audio, models and video.
5. Set up **Smart Merge** for scenes and prefabs, in `.git/config` or your global config:

```
[merge]
    tool = unityyamlmerge
[mergetool "unityyamlmerge"]
    trustExitCode = false
    cmd = 'C:/Program Files/Unity/Hub/Editor/<version>/Editor/Data/Tools/UnityYAMLMerge.exe' merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"
```

then `git mergetool` on a conflicted scene.

6. Avoid two people editing the same scene at once; split levels into several scenes or use prefabs so changes land in different files.
7. Close Unity (or at least don't let it save) while switching branches or pulling big changes, then let it reimport.

## What are Git submodules?

A submodule is another Git repository placed inside yours at a fixed commit, for example a shared library or a package used by several games:

```
git submodule add https://github.com/someone/shared-tools.git Packages/shared-tools
git commit -m "Add shared-tools as a submodule"

git clone --recurse-submodules https://github.com/you/MyGame.git   # clone with submodules
git submodule update --init --recursive                              # in an existing clone
git submodule update --remote                                        # move submodules to their latest commit
```

Your repository records only which commit of the submodule to use; updating it is a deliberate commit. Submodules are easy to get wrong (forgetting `--recurse-submodules`, committing inside a detached submodule), so for Unity, a Unity package referenced by Git URL in `Packages/manifest.json` is often simpler.

## Git warns 'LF will be replaced by CRLF'. Is that a problem?

No. It's Git telling you it will convert line endings: Windows uses CRLF (`\r\n`), Linux and macOS use LF (`\n`). With `core.autocrlf true` (the Windows default in Git for Windows), Git stores LF in the repository and gives you CRLF on checkout, so files look normal on every system.

To make line endings consistent for everyone regardless of their settings, add a `.gitattributes` file:

```
* text=auto
*.cs text diff=csharp
*.sh text eol=lf
*.bat text eol=crlf
*.png binary
*.unity text eol=lf merge=unityyamlmerge
*.prefab text eol=lf merge=unityyamlmerge
*.asset text eol=lf merge=unityyamlmerge
```

The `merge=unityyamlmerge` part tells Git to merge scenes, prefabs and assets with Unity's Smart Merge tool instead of line by line. It only works once that **merge driver** is defined in your Git config, since `.gitattributes` just names it. Add this to your global config (`C:\Users\<you>\.gitconfig`, or the repository's `.git/config`), with the path of your Unity version:

```
[merge "unityyamlmerge"]
    name = Unity SmartMerge (UnityYAMLMerge)
    driver = 'C:/Program Files/Unity/Hub/Editor/<version>/Editor/Data/Tools/UnityYAMLMerge.exe' merge -p %O %B %A %A
    recursive = binary
```

`%O` is the common ancestor, `%B` the other branch's version, and `%A` yours, where the result is written. If the tool can't merge a file cleanly, Git reports a conflict as usual. Git config isn't committed, so everyone on the team adds this on their own computer; without it, Git silently merges those files line by line as plain text (no warning), so check `git config --get merge.unityyamlmerge.driver` prints the path. Upgrading Unity changes the path, so update it then.

If a whole file shows as changed but you only touched one line, line endings are the usual cause; `git diff --ignore-cr-at-eol` confirms it, and `git add --renormalize .` after adding `.gitattributes` fixes the repository once.

## Git says 'Your local changes would be overwritten by merge/checkout'. What do I do?

Git refuses to switch branches or pull because it would overwrite edits you haven't committed. Choose one:

```
git add . && git commit -m "WIP"     # keep them: commit first
git stash                            # shelve them, do the pull/switch, then: git stash pop
git restore .                        # throw them away (can't be undone)
```

For a single file you don't care about (a generated file, say): `git restore that-file`. In GitHub Desktop the same choice appears as a dialog: bring the changes to the other branch, stash them, or cancel.

## Git says 'not a git repository' or 'refusing to merge unrelated histories'. What do these mean?

- **`fatal: not a git repository (or any of the parent directories): .git`**: you're running Git in a folder that isn't inside a repository. `cd` into the project folder (the one with the hidden `.git` folder), or run `git init` if it should become one.
- **`fatal: refusing to merge unrelated histories`**: the two branches have no commit in common, typically when a local project and a new GitHub repository were each given a first commit (for example GitHub created a README). If that's expected, allow it once:

```
git pull origin main --allow-unrelated-histories
```

then resolve any conflicts (often just README or .gitignore) and push.

## How do I remove a file from Git history that should never have been committed (like a password or a huge file)?

Deleting the file in a new commit doesn't remove it from history. To erase it everywhere:

1. **If it's a secret: revoke or change it first** (new API key, new password). Once pushed, assume it was seen; bots scan GitHub for keys within minutes.
2. Rewrite history with **git filter-repo** (`pip install git-filter-repo`; the older `git filter-branch` is slow and error-prone):

```
git filter-repo --invert-paths --path config/secrets.json
git filter-repo --strip-blobs-bigger-than 100M
```

3. Re-add the remote if filter-repo removed it, then force-push every branch and tag: `git push --force --all` and `git push --force --tags`.
4. Everyone else must re-clone (their copies still contain the old history).

GitHub's **push protection** blocks many secrets before they're pushed; add the file to `.gitignore` so it doesn't happen again.

## How can I make Git commands shorter with aliases?

```
git config --global alias.st status
git config --global alias.co checkout
git config --global alias.sw switch
git config --global alias.lg "log --oneline --graph --decorate --all"
git config --global alias.undo "reset --soft HEAD~1"
git config --global alias.amend "commit --amend --no-edit"
```

Now `git lg` shows the branch graph and `git undo` takes back the last commit (keeping its changes staged). Aliases live in your global config (`~/.gitconfig`, which on Windows is `C:\Users\<you>\.gitconfig`).

## What is git worktree?

A worktree is a second working folder attached to the same repository, checked out on a different branch, so you can work on two branches at once without stashing or switching:

```
git worktree add ../MyGame-hotfix hotfix      # check out branch hotfix in another folder
git worktree add -b experiment ../MyGame-exp  # new branch in a new folder
git worktree list
git worktree remove ../MyGame-hotfix
```

Both folders share one `.git` history (no second download). A branch can only be checked out in one worktree at a time. For Unity projects, each worktree needs its own `Library/` import, which takes time the first time it opens.

## What are Git hooks?

Hooks are scripts Git runs at certain moments, stored in `.git/hooks/` (named `pre-commit`, `commit-msg`, `pre-push` and so on). If a `pre-commit` hook exits with an error, the commit is cancelled, which makes them useful for automatic checks:

```
#!/bin/sh
# .git/hooks/pre-commit: refuse commits with debug leftovers
if git diff --cached | grep -q "Debug.Break()"; then
  echo "Remove Debug.Break() before committing"
  exit 1
fi
```

Files in `.git/hooks` aren't committed, so they don't reach other people. To share hooks, keep them in a committed folder and point Git at it with `git config core.hooksPath .githooks`, or use a tool like **pre-commit** (Python) or **Husky** (JavaScript). Checks that must always run belong in CI (GitHub Actions) instead, since hooks can be skipped with `--no-verify`.

# GitHub

## How do I create a repository on GitHub?

1. Sign in at github.com, click **+** (top right) → **New repository**.
2. Choose the owner (you or an organisation), a name, and **Public** (anyone can see it) or **Private** (only you and people you invite).
3. Optional: add a README, a `.gitignore` template (pick **Unity** for a Unity project) and a license. Leave them all unticked if you're about to push an existing project, so the first push doesn't conflict.
4. **Create repository**. GitHub then shows the commands to push an existing folder, or a **Set up in Desktop** button.

Private repositories are free, with unlimited collaborators. You can rename, transfer, archive or change the visibility later under the repository's **Settings**.

## What is a pull request on GitHub, and how does the workflow go?

A **pull request (PR)** proposes merging one branch into another (usually a feature branch into `main`) and gives a place to review and discuss the change before it's merged.

1. Make a branch, commit, and push it: `git push -u origin new-menu`.
2. On GitHub: **Compare & pull request** (the banner after pushing), or Pull requests → New pull request. Choose the base branch (`main`) and the compare branch (`new-menu`), write a title and description (what changed, why, how it was tested), and **Create pull request**. Mark it as a **draft** if it isn't ready.
3. Reviewers comment on lines, suggest changes, and **Approve** or **Request changes**. Automated checks (GitHub Actions) run on every push and show as passing or failing.
4. More commits pushed to the same branch appear in the PR automatically.
5. When it's approved and the checks pass: **Merge pull request**. Then **Delete branch**.

Writing `Fixes #12` in the description closes issue 12 automatically when the PR is merged.

## What's the difference between merge, squash and rebase when merging a pull request on GitHub?

The **Merge pull request** button has three options (the repository's settings can allow or forbid each):

- **Create a merge commit**: all the branch's commits are kept, plus a merge commit tying them in. Full history; shows exactly what happened, but it can look busy.
- **Squash and merge**: all the branch's commits become **one** commit on `main`. A clean, one-commit-per-feature history; the individual work-in-progress commits disappear from `main` (they stay visible in the closed PR).
- **Rebase and merge**: the branch's commits are replayed onto `main` one by one, with no merge commit. A straight history that keeps each commit.

Squash is popular for small teams and messy branches; merge commits suit long-lived branches where each commit matters. After squash or rebase merging, the branch's original commits are no longer ancestors of `main`, so delete the branch (or reset it to `main`) instead of continuing to commit on it, or the next PR will show the old changes again.

## How do code reviews work on GitHub?

On a pull request's **Files changed** tab:

- Hover over a line and click **+** to comment on it; drag to comment on several lines.
- **Suggest a change**: the suggestion button in a comment proposes replacement code that the author can apply with one click (**Commit suggestion**).
- **Start a review** collects several comments and sends them together, ending with **Comment**, **Approve** or **Request changes**.
- Mark files as **Viewed** to track progress through a big PR.

As the author, reply to comments, push fixes to the same branch, and **Resolve conversation** when a thread is dealt with. **Re-request review** asks the reviewer to look again.

Repository rules (Settings → Branches/Rules) can require an approval and passing checks before anything merges into `main`. Good PRs are small and focused, with a description that says what to look at.

## How do issues and projects work on GitHub?

**Issues** track bugs, ideas and tasks for a repository:

- Title and description (Markdown, screenshots, checklists `- [ ] item`), **labels** (bug, enhancement...), **assignees** and **milestones**.
- Refer to issues anywhere with `#12`; mention people with `@name`.
- Close them from a commit or PR: `Fixes #12`, `Closes #12` or `Resolves #12` in the PR description or a commit message pushed to the default branch.
- **Issue templates** (`.github/ISSUE_TEMPLATE/`) give bug reports a standard form.

**Projects** are boards and tables built from issues and pull requests (columns like Todo / In progress / Done, custom fields, views), good for planning a game's features across one or more repositories.

**Discussions** (enable in Settings) are for questions and ideas that aren't tasks.

## What is GitHub Actions? Show a simple workflow.

GitHub Actions runs automated jobs (build, test, deploy) on GitHub's machines when something happens in the repository: a push, a pull request, a schedule, a button press. Workflows are YAML files in `.github/workflows/`.

A workflow that builds and tests a .NET project on every push and pull request:

```yaml
# .github/workflows/test.yml
name: Tests
on:
  push:
    branches: [main]
  pull_request:
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "8.0.x"
      - run: dotnet build --configuration Release
      - run: dotnet test --configuration Release --no-build
```

And one for Python:

```yaml
name: Python tests
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-python@v5
        with:
          python-version: "3.12"
      - run: pip install -r requirements.txt pytest
      - run: pytest
```

Results show on the commit and the pull request (green tick or red cross), and the **Actions** tab has the logs. Public repositories get Actions free; private ones get a monthly allowance of minutes. Unity builds need a license and a lot of setup; the community **GameCI** actions (game.ci) handle that.

## How do I keep secrets like API keys out of a GitHub repository?

- Never commit them. Put them in a file that's in `.gitignore` (`.env`, `secrets.json`), or in environment variables, and commit an example file without real values (`.env.example`).
- For GitHub Actions, store them in **Settings → Secrets and variables → Actions** and use them as `${{ secrets.MY_KEY }}`: they're encrypted and hidden in logs.
- GitHub's **secret scanning** and **push protection** detect many kinds of keys and block the push; if it blocks you, remove the key from the commit (don't bypass it).
- If a secret was pushed: **revoke it first** (make a new key), then remove it from history if needed (`git filter-repo`).
- Use fine-grained **personal access tokens** with only the permissions and repositories a tool needs, and an expiry date.

## How do GitHub Releases work?

A **Release** is a named version of your project built on a Git tag, with notes and downloadable files, which is the usual way to publish builds of a game or tool:

1. Repository page → **Releases** → **Draft a new release**.
2. Choose or create a tag (for example `v1.2.0`) and the branch or commit it points to.
3. Title and notes (**Generate release notes** lists the merged PRs since the last release).
4. Attach files: the game's `.zip`, the `.exe` installer, an `.apk` (each up to 2 GB).
5. Tick **Set as a pre-release** for betas, then **Publish release**.

Releases can also be created automatically by a GitHub Actions workflow when a tag is pushed. Use semantic versioning (MAJOR.MINOR.PATCH): bump PATCH for fixes, MINOR for new features, MAJOR for breaking changes.

## What should a good README on GitHub contain?

The README (`README.md` in the repository's root) is the first thing visitors see. Include:

1. **What it is**, in one or two sentences, with a screenshot or GIF for a game.
2. **How to install or run it**: requirements (Unity version, .NET, Python), the exact commands or steps.
3. **How to use it**: a quick example.
4. **How to build and test it**, for contributors.
5. **Project status**, the **license**, and where to ask questions or report bugs.

It's Markdown: `#` headings, `**bold**`, lists, links `[text](url)`, images `![alt](docs/screenshot.png)`, code blocks with three backticks and a language name, and tables. A file named `README.md` inside any folder is shown when you browse that folder on GitHub.

## How do branch protection and rulesets work on GitHub?

They stop mistakes on important branches like `main`. In the repository's **Settings → Rules → Rulesets** (or the older Settings → Branches → Branch protection rules) you can require:

- **A pull request before merging** (no direct pushes), with a number of **approvals**;
- **Status checks to pass** (your GitHub Actions tests);
- **Up-to-date branches** before merging;
- **Linear history** (squash or rebase merges only);
- **No force pushes** and **no deletion** of the branch;
- **Signed commits**.

Admins can be allowed to bypass, or not. Even for a solo project, blocking force pushes and deletion on `main` is a cheap safety net.

## What is the GitHub CLI (gh) and what can it do?

`gh` is GitHub's command-line tool (cli.github.com; `winget install GitHub.cli` on Windows). Sign in once with `gh auth login`. Then:

```
gh repo create MyGame --private --source . --push   # make a GitHub repo from the current folder and push
gh repo clone owner/repo
gh pr create --fill                     # open a PR for the current branch (title/body from the commits)
gh pr list
gh pr checkout 42                       # check out PR 42's branch locally
gh pr view 42 --web
gh pr merge 42 --squash --delete-branch
gh issue create --title "Jump bug" --body "..."
gh issue list --label bug
gh run list                             # GitHub Actions runs
gh run watch                            # follow the current run live
gh release create v1.2.0 build/MyGame.zip --generate-notes
```

It's handy for scripts and for doing GitHub tasks without leaving the terminal.

## What are GitHub Pages and GitHub Codespaces?

- **GitHub Pages** hosts a static website straight from a repository, free: Settings → Pages → choose a branch and folder (for example `main` and `/docs`), and the site appears at `https://you.github.io/repo/`. Good for documentation, a game's homepage, or a WebGL build of a Unity game (with compression set to a format the browser can load without special server headers, such as disabled compression or gzip with the decompression fallback).
- **Codespaces** are cloud development machines: **Code → Codespaces → Create** opens the repository in VS Code in the browser (or your desktop VS Code), with the tools installed from a `.devcontainer` configuration. Personal accounts get a free monthly allowance of hours; useful for working from another computer, less so for Unity, which needs a real desktop.

## How do I invite collaborators to a GitHub repository?

Repository → **Settings → Collaborators** (or **Collaborators and teams**) → **Add people**, then enter their GitHub username or email. They get an invitation and, once they accept, can push to the repository.

For a personal repository, collaborators get write access. Organisations have finer roles (Read, Triage, Write, Maintain, Admin) and teams. To accept contributions from anyone without giving them write access, let them **fork** the repository and send pull requests instead.

Remove someone the same way (Settings → Collaborators → Remove). Combine with branch protection so even collaborators go through pull requests for `main`.

## What's a good branching workflow for a small game project on GitHub?

**GitHub flow** works well for one person or a small team:

1. `main` always builds and runs.
2. For each change (feature, fix, experiment), make a short-lived branch from `main` with a clear name: `feature/double-jump`, `fix/menu-crash`.
3. Commit small steps on the branch; push it to GitHub (backup, and others can see it).
4. Open a pull request into `main`; let the automated tests run; review it (yourself, a teammate, or an AI reviewer).
5. Merge (squash merging keeps `main` tidy) and delete the branch.
6. Tag releases (`v0.3.0`) from `main` and publish builds as GitHub Releases.

Habits that save trouble: pull `main` before branching, keep branches short (days, not months), merge `main` into long branches regularly, and never commit Unity's `Library/` or build folders.

## Why is a file over 100 MB rejected by GitHub, and what are GitHub's size limits?

GitHub blocks pushes containing any file larger than **100 MB** and warns about files over **50 MB**. Repositories should ideally stay under a few GB (GitHub recommends under 1 GB; above about 5 GB it may contact you). Release attachments can be up to 2 GB each.

If a push is rejected for a big file:

1. Track that file type with **Git LFS** (`git lfs track "*.fbx"`), or keep the file out of Git (`.gitignore`) and share it another way.
2. Because the file is already in a commit, move it out of history: `git lfs migrate import --include="*.fbx" --everything` (puts existing ones into LFS), or remove it with `git filter-repo --strip-blobs-bigger-than 100M`.
3. Push again (a history rewrite needs `--force-with-lease`).

Unity builds, `Library/`, and videos are the usual culprits.

# GitHub Desktop

## What is GitHub Desktop and how do I set it up?

GitHub Desktop is GitHub's free Git app for Windows and macOS (desktop.github.com). It does everyday Git with buttons: clone, commit, branch, push, pull, merge, pull requests, and shows every change as a readable diff. It includes its own Git, so nothing else needs installing.

Setup:

1. Install it and sign in with your GitHub account (**File → Options → Accounts** if you skipped it). Signing in handles authentication: no passwords or tokens to type.
2. **File → Options → Git**: check your name and email (they go into your commits) and the default branch name (`main`).
3. **File → Options → Integrations**: choose your external editor (VS Code, Visual Studio, Rider) and shell, so **Repository → Open in ...** works.

The main window: **Current repository** and **Current branch** dropdowns at the top, the **Fetch origin / Pull / Push** button next to them, and the **Changes** and **History** tabs on the left.

## How do I clone or add a repository in GitHub Desktop?

- **Clone from GitHub**: **File → Clone repository** (Ctrl+Shift+O). Pick one of your repositories from the list (GitHub.com tab) or paste a URL (URL tab), choose a local folder, **Clone**.
- **A folder that's already a Git repository**: **File → Add local repository** (Ctrl+O) and pick the folder.
- **A new repository**: **File → New repository** (Ctrl+N): name, local path, optional README, `.gitignore` template (choose **Unity**) and license.
- **Put a local repository on GitHub**: click **Publish repository**, choose the name and whether it's private.

If you clone a **fork**, Desktop asks whether you'll contribute to the parent project or use it for your own purposes; it uses that to choose where new pull requests go.

## How do I commit changes in GitHub Desktop?

1. Open the **Changes** tab: every changed file is listed with a checkbox, and clicking a file shows its diff.
2. Untick files you don't want in this commit. To commit only **some lines** of a file, click the line numbers in the diff to exclude or include individual lines or blocks.
3. At the bottom left, write a **Summary** (required, the short first line) and an optional **Description**.
4. Click **Commit to main** (the button names the current branch).
5. Click **Push origin** at the top to upload the commit to GitHub.

Right after committing, an **Undo** button appears at the bottom for the last commit (as long as it isn't pushed): it takes the commit back and leaves the changes in the Changes tab. Right-click a file for **Discard changes** (throws away edits; they go to the Recycle Bin) or **Ignore file** (adds it to `.gitignore`).

## What do Fetch, Pull and Push do in GitHub Desktop?

The button next to the branch name changes with the situation:

- **Fetch origin**: checks GitHub for new commits without changing your files. Desktop also fetches by itself every few minutes.
- **Pull origin** (with a down arrow and a number): GitHub has commits you don't; click to download and merge them into your branch.
- **Push origin** (with an up arrow and a number): you have commits GitHub doesn't; click to upload them.
- **Publish branch**: the branch only exists on your computer; click to create it on GitHub.

If both arrows show numbers, pull first, then push. If someone pushed while you were working, Desktop says your push was rejected because there are newer commits on the remote; pull, then push again.

## How do I get the latest changes from GitHub into my local Unity project with GitHub Desktop?

1. **Close Unity** (or at least make sure it isn't saving), so it doesn't write over files while they change.
2. In GitHub Desktop, pick the repository and the branch you work on (usually `main`).
3. Click **Fetch origin**; if it turns into **Pull origin** with a number, click it.
4. If you had uncommitted changes that touch the same files, Desktop asks what to do: commit them first, or **stash** them, pull, and restore the stash afterwards.
5. Open the project in Unity: it reimports the changed assets and recompiles the scripts. Check the Console for errors.

If the changes were made on another branch (for example by a pull request), switch to that branch with **Current branch** → pick it, or wait until it's merged into `main` and pull `main`. Remember files outside `Assets` too: tools, executables and READMEs that changed come down with the pull as well.

## How do I work with branches in GitHub Desktop?

- **Create**: **Current branch → New branch** (Ctrl+Shift+N), name it, and choose what to base it on (usually `main`). If you have uncommitted changes, Desktop asks whether to bring them to the new branch or leave them on the current one.
- **Switch**: **Current branch** → click a branch. Uncommitted changes either come with you or get stashed (Desktop asks).
- **Publish**: **Publish branch** pushes it to GitHub for the first time.
- **Update it from main**: **Branch → Update from main** merges the latest `main` into your branch.
- **Merge another branch into this one**: **Branch → Merge into current branch** (Ctrl+Shift+M), choose the branch; the dialog shows how many commits come in and whether there will be conflicts. **Squash and merge** turns them into one commit.
- **Rebase**: **Branch → Rebase current branch** (Ctrl+Shift+E) replays your commits on top of another branch (for branches only you use).
- **Rename / Delete**: **Branch → Rename** or **Delete** (deleting also offers to delete it on GitHub).
- **Compare**: **Branch → Compare to branch** shows the commits one branch has that another doesn't.

## How do I resolve a merge conflict in GitHub Desktop?

When a merge, pull or rebase hits a conflict, Desktop shows a dialog listing the **conflicted files**:

1. For each file, click **Open in Visual Studio Code** (or your editor). The file contains both versions between `<<<<<<<`, `=======` and `>>>>>>>` markers; in VS Code use **Accept Current Change**, **Accept Incoming Change** or **Accept Both**, or edit by hand, then save.
2. Or, for a whole file, use the arrow next to it to pick one side: **Use the modified file from main** or **from the other branch**.
3. Back in Desktop, the file shows as resolved once no markers remain.
4. When all are resolved, click **Continue merge** (or **Continue rebase**). **Abort merge** puts everything back as it was.

For Unity scenes and prefabs, prefer picking one whole side, or Unity's Smart Merge from the command line (`git mergetool` with UnityYAMLMerge), rather than editing YAML by hand.

## How do I create a pull request from GitHub Desktop?

1. Commit on a branch (not `main`) and click **Publish branch** (or **Push origin**).
2. **Branch → Create pull request** (Ctrl+R), or the **Create Pull Request** button that appears after pushing. It opens github.com with the pull request form filled in for your branch; add a description and click **Create pull request**.
3. **Branch → Preview Pull Request** shows, inside Desktop, the changes the pull request would contain before you create it.

Once it exists, Desktop shows the pull request's checks (passing or failing) on the branch. **Current branch → Pull requests** lists open pull requests in the repository, and you can check one out to try it locally.

## How do I undo, revert or amend commits in GitHub Desktop?

In the **History** tab:

- **Undo the last commit** (not pushed yet): the **Undo** button that appears under the commit box right after committing. The changes go back to the Changes tab.
- **Amend the last commit**: right-click it → **Amend commit**, adjust the files or message, and commit again. If it was already pushed, Desktop warns that you'll need to force push.
- **Revert a commit** (safe for pushed commits): right-click → **Revert changes in commit**. Desktop makes a new commit that undoes it.
- **Squash commits**: drag one commit onto another, or select several (Ctrl+click) → right-click → **Squash**; then write the combined message.
- **Reorder commits**: drag a commit up or down in the list.
- **Cherry-pick**: drag a commit onto a branch in the **Current branch** list, or right-click → **Cherry-pick commit**.
- **Create a branch or tag from an old commit**: right-click → **Create branch from commit** / **Create tag**.

Rewriting commits (squash, reorder, amend) on a branch that's already pushed needs a force push, which Desktop offers with a warning; avoid it on branches others use.

## How do I stash changes in GitHub Desktop?

**Branch → Stash all changes** (or switch branch and choose **Leave my changes on this branch** / stash when asked). The changes disappear from the Changes tab and a **Stashed changes** entry appears at the bottom of the Changes list.

Click **Stashed changes** to see them, then **Restore** to bring them back onto the current branch, or **Discard** to delete them. Desktop keeps one stash per branch, unlike the command line's list of stashes (`git stash list`), so restore or discard a stash before making another on the same branch.

## How do I edit .gitignore and repository settings in GitHub Desktop?

**Repository → Repository settings**:

- **Remote**: the URL of `origin` (to point the repository at a different GitHub repository).
- **Ignored files**: edit `.gitignore` directly. You can also right-click a file in the Changes tab → **Ignore file** or **Ignore all .xyz files**.
- **Git config**: use your global name and email, or set different ones for this repository (for example a work email).
- **Fork behavior**, for forks.

Other useful items in the **Repository** menu: **Open in Visual Studio Code**, **Show in Explorer**, **Open in Command Prompt / PowerShell** (for anything Desktop doesn't do, like `git lfs` commands or `git bisect`), and **View on GitHub**.

## What can't GitHub Desktop do, so I need the command line?

Desktop covers everyday work. For these, use the command line (**Repository → Open in Command Prompt**, then `git` commands) on the same repository; Desktop picks up the result:

- `git reflog` to recover lost commits, and `git reset --hard` to a specific commit;
- `git bisect` to find which commit broke something;
- interactive rebase with full control (`git rebase -i`), `git filter-repo` to remove files from history;
- `git lfs` setup and migration (`git lfs track`, `git lfs migrate`), and submodule commands;
- `git mergetool` (Unity Smart Merge), `git worktree`, hooks and aliases;
- `git clean` to delete untracked files, `git blame` (the GitHub website also has a Blame view);
- several stashes per branch (`git stash list`).

GitHub Desktop and the command line can be mixed freely: both read and write the same `.git` folder.

## GitHub Desktop says 'Newer commits on remote' or shows a push was rejected. What do I do?

It means GitHub has commits on your branch that you don't have yet, often from another computer, a teammate, or a bot or AI assistant pushing to the same branch:

1. Click **Pull origin** (or **Fetch origin**, then **Pull origin**) to bring them in. If both sides changed the same lines, resolve the conflict and **Continue merge**.
2. Then **Push origin**.

Don't choose force push here; it would delete the other commits from GitHub. Force push is only for your own branch after squashing, reordering or amending commits you'd already pushed.

## My GitHub Desktop shows hundreds of changed files I didn't touch in my Unity project. Why?

Usually one of these:

- **Unity-generated folders aren't ignored**: `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/` and the `.csproj`/`.sln` files. Add a Unity `.gitignore` (**Repository → Repository settings → Ignored files**), then if they were already committed, remove them from tracking from the command line: `git rm -r --cached Library` and commit.
- **Line endings**: files changed only from LF to CRLF. A `.gitattributes` with `* text=auto` and `git add --renormalize .` fixes it once.
- **Unity re-saved assets**: opening a project in a newer Unity version, or reimporting, rewrites `.meta`, scene and asset files. Commit them as one "Upgrade to Unity 6.2" commit, or discard them if the change wasn't intended.
- **Asset Serialization changed** to Force Text: every scene and prefab is rewritten once; commit that by itself.

The diff on the right shows what changed in each file, which tells you which case it is.

