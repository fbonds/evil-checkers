# Contributing to Entropy Checkers

Thank you for your interest in contributing to Entropy Checkers! This document provides guidelines for contributing to the project.

## Table of Contents

1. [Getting Started](#getting-started)
2. [Development Setup](#development-setup)
3. [Code Style](#code-style)
4. [Making Changes](#making-changes)
5. [Pull Request Process](#pull-request-process)
6. [Reporting Issues](#reporting-issues)

---

## Getting Started

### Prerequisites

- **Unity 6** (version 6000.3.5f1 or compatible)
- **Git** for version control
- A code editor (Visual Studio, VS Code, or Rider recommended)

### Project Overview

Entropy Checkers follows a layered architecture:

| Layer | Purpose | Unity Dependency |
|-------|---------|------------------|
| Core | Game logic, data structures | None (pure C#) |
| Game | Rules, move generation | None (pure C#) |
| AI | Computer opponent | None (pure C#) |
| Presentation | Rendering, input, UI | Yes (MonoBehaviours) |

**Key principle**: Keep game logic in pure C# classes. Only the Presentation layer should use MonoBehaviours.

---

## Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/[username]/evil-checkers.git
   cd evil-checkers
   ```

2. **Open in Unity**
   - Launch Unity Hub
   - Click "Open" and select the project folder
   - Wait for Unity to import assets

3. **Open the main scene**
   - Navigate to `Assets/Scenes/`
   - Open `SampleScene.unity` (or the main game scene once created)

---

## Code Style

### General Guidelines

- Use **PascalCase** for public members, types, and methods
- Use **camelCase** for private fields and local variables
- Prefix private fields with underscore: `_privateField`
- One class per file (exceptions for small related types)
- Keep methods short and focused (< 30 lines preferred)

### C# Conventions

```csharp
namespace EntropyCheckers.Core
{
    public class ExampleClass
    {
        // Constants first
        public const int MaxValue = 100;
        
        // Private fields with underscore prefix
        private int _count;
        private List<Item> _items;
        
        // Public properties
        public int Count => _count;
        
        // Events
        public event Action<int> OnCountChanged;
        
        // Constructor
        public ExampleClass()
        {
            _items = new List<Item>();
        }
        
        // Public methods
        public void DoSomething()
        {
            // Implementation
        }
        
        // Private methods
        private void HelperMethod()
        {
            // Implementation
        }
    }
}
```

### Comments

- Avoid obvious comments
- Document "why" not "what"
- Use XML docs for public APIs when helpful

```csharp
// Bad: Increment the counter
_count++;

// Good: Reset after max to prevent overflow in long games
if (_count >= MaxValue) _count = 0;
```

### Unity-Specific

- Use `[SerializeField]` for inspector-exposed private fields
- Avoid `Find` methods in Update loops
- Cache component references in `Awake()` or `Start()`

---

## Making Changes

### Branch Naming

- `feature/description` - New features
- `fix/description` - Bug fixes
- `refactor/description` - Code improvements
- `docs/description` - Documentation updates

### Commit Messages

Use clear, descriptive commit messages:

```
Add MoveGenerator with capture chain detection

- Implement recursive capture pathfinding
- Add unit tests for multi-jump scenarios
- Handle edge cases for board boundaries
```

### Testing

- Write unit tests for Core/Game/AI layer changes
- Test in Unity Editor for Presentation layer changes
- Verify no console errors or warnings

---

## Pull Request Process

1. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make your changes**
   - Follow the code style guidelines
   - Add tests if applicable
   - Update documentation if needed

3. **Test your changes**
   - Run existing tests
   - Test manually in Unity Editor

4. **Commit your changes**
   ```bash
   git add .
   git commit -m "Description of changes"
   ```

5. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Open a Pull Request**
   - Provide a clear description of changes
   - Reference any related issues
   - Include screenshots for visual changes

### PR Checklist

- [ ] Code follows project style guidelines
- [ ] No new warnings or errors
- [ ] Tests pass (if applicable)
- [ ] Documentation updated (if applicable)
- [ ] Commits are clean and well-described

---

## Reporting Issues

### Bug Reports

Include:
- Unity version
- Steps to reproduce
- Expected behavior
- Actual behavior
- Screenshots if applicable

### Feature Requests

Include:
- Clear description of the feature
- Use case / why it's valuable
- Any implementation ideas (optional)

### Issue Labels

| Label | Description |
|-------|-------------|
| `bug` | Something isn't working |
| `enhancement` | New feature request |
| `documentation` | Documentation improvements |
| `good first issue` | Good for newcomers |
| `help wanted` | Extra attention needed |

---

## Project Roadmap

See the README for current development status. Priority areas:

1. **Core gameplay** - Board rendering, move validation, basic game loop
2. **AI opponent** - Minimax implementation with difficulty levels
3. **Polish** - UI, animations, sound effects
4. **Extended features** - Save/load, statistics, achievements

---

## Questions?

Feel free to open an issue with the `question` label if you need clarification on anything.

Thank you for contributing to Entropy Checkers!
