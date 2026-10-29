# C# and .NET: questions and answers

The readable version of `extra/csharp.jsonl` (the same content, which `build_dataset.py` adds to the training data).

## What is the difference between value types and reference types in C#?

Value types (`int`, `double`, `bool`, `char`, `struct`s, `enum`s) hold their data directly. Assigning one copies the whole value, so changing the copy doesn't affect the original.

Reference types (`class`es, `string`, arrays, delegates, `interface` instances) hold a reference to an object on the heap. Assigning one copies the reference, so both variables point at the same object.

```csharp
struct PointS { public int X; }
class  PointC { public int X; }

var a = new PointS { X = 1 }; var b = a; b.X = 5;   // a.X is still 1
var c = new PointC { X = 1 }; var d = c; d.X = 5;   // c.X is now 5 (same object)
```

Value types can't be `null` unless they're nullable (`int?`). Reference types can be `null`.

## What does `var` do in C#?

`var` lets the compiler infer a local variable's type from the value it's initialised with. The variable is still strongly typed: `var` is not `dynamic`.

```csharp
var count = 10;                      // int
var names = new List<string>();      // List<string>
var pair = (Name: "Pikachu", Hp: 60); // a named tuple
```

It only works for local variables that are initialised in the same statement. Use it when the type is obvious from the right-hand side; write the type out when it isn't.

## What's the difference between `const` and `readonly`?

- `const` is a compile-time constant. It must be a literal-like value (numbers, `string`, `bool`, `char`, `enum`, `null`), is implicitly `static`, and its value is copied into every place that uses it at compile time.
- `readonly` is a field that can only be assigned in its declaration or in a constructor. Its value is decided at runtime and it can be any type.

```csharp
public const int MaxBench = 5;                       // fixed forever
public readonly DateTime Created = DateTime.Now;     // set once, at runtime
public static readonly List<string> Names = new();   // the reference is fixed, the list isn't
```

Note that `readonly` on a reference-type field only stops you replacing the reference; the object itself can still change. Prefer `static readonly` over `const` for values that might change between versions of a library, because other assemblies bake `const` values in when they compile.

## How do nullable value types work in C#?

`T?` for a value type (`int?`, `bool?`) is shorthand for `Nullable<T>`: it can hold any `T` value or `null`.

```csharp
int? hp = null;
if (hp.HasValue) Console.WriteLine(hp.Value);
int safe = hp ?? 0;          // 0 when hp is null
int? doubled = hp * 2;       // null propagates: still null
```

Useful operators: `??` (use a fallback when null), `??=` (assign only if null) and `?.` (call a member only if the object isn't null). Reading `.Value` when there is no value throws `InvalidOperationException`.

## What are nullable reference types (`string?`) in C# 8+?

With `<Nullable>enable</Nullable>` (or `#nullable enable`), the compiler tracks whether reference-type variables might be `null`:

- `string` means "never null"; the compiler warns if you might assign `null` to it.
- `string?` means "might be null"; the compiler warns if you dereference it without checking.

```csharp
#nullable enable
string? FindName(int id) => id == 1 ? "Eevee" : null;

var name = FindName(2);
Console.WriteLine(name.Length);     // warning: possible null dereference
if (name != null) Console.WriteLine(name.Length); // fine
```

It only produces warnings; it doesn't change the runtime. `!` (the null-forgiving operator, `name!.Length`) tells the compiler "trust me", so use it sparingly.

## How do string interpolation and verbatim strings work in C#?

- Interpolated strings start with `$` and put expressions in braces: `$"{name} has {hp} HP"`. Format specifiers go after a colon (`$"{price:F2}"`, `$"{ratio:P0}"`) and alignment after a comma (`$"{name,-10}|"`).
- In a normal string, a backslash starts an escape (`\n`, `\t`), so a Windows path needs doubled backslashes: `"C:\\Games\\Pokemon"`. Verbatim strings start with `@` and ignore escapes, so the same path is `@"C:\Games\Pokemon"`. Double a quote to include one: `@"He said ""hi"" to me"`.
- They combine: `$@"{folder}\save.txt"`.
- C# 11 raw string literals start and end with three (or more) double quotes and need no escaping at all, which suits JSON and other text full of quotes.

## Why are strings immutable in C# and when should I use StringBuilder?

A `string` can't be changed after it's created. Every `+`, `Replace` or `Substring` makes a new string, which is safe (strings can be shared freely and used as dictionary keys) but costs an allocation each time.

Building a string in a loop with `+=` therefore creates a new string every iteration (quadratic work for long strings). Use `StringBuilder` instead:

```csharp
var sb = new StringBuilder();
foreach (var card in deck)
    sb.Append(card.Name).Append('\n');
string text = sb.ToString();
```

For a handful of concatenations, `+` or interpolation is fine and more readable; `string.Join(", ", items)` is best for joining a collection.

## What's the difference between `==` and `.Equals()` for strings in C#?

For `string`, both compare the characters (C# overloads `==` for strings), so `a == b` and `a.Equals(b)` give the same result. Differences:

- `a.Equals(b)` throws `NullReferenceException` if `a` is null; `a == b` and `string.Equals(a, b)` don't.
- For case-insensitive or culture-aware comparisons, pass a `StringComparison`:

```csharp
string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
a.StartsWith("Pika", StringComparison.Ordinal);
```

Use `Ordinal`/`OrdinalIgnoreCase` for identifiers, keys and file names (fast and predictable), and culture-aware comparisons only for text shown to and sorted for users.

## Why should I use CultureInfo.InvariantCulture when parsing or formatting numbers?

`double.Parse`, `ToString()` and string interpolation use the current culture by default. On a PC set to German or French, the decimal separator is a comma, so `"1.5"` fails to parse or turns into 15, and `1.5.ToString()` writes `"1,5"`.

For anything written to files, sent over a network or read by another program, use the invariant culture:

```csharp
using System.Globalization;

double d = double.Parse("1.5", CultureInfo.InvariantCulture);
string s = d.ToString("R", CultureInfo.InvariantCulture);
bool ok = int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int n);
```

Keep current-culture formatting for text shown to the user.

## What does `TryParse` do and why is it better than `Parse`?

`int.Parse("abc")` throws a `FormatException` on bad input. `int.TryParse` returns `false` instead and gives the result through an `out` parameter, which avoids using exceptions for normal control flow:

```csharp
if (int.TryParse(input, out int games) && games > 0)
    Train(games);
else
    Console.WriteLine("Please type a positive number.");
```

Every numeric type, `bool`, `DateTime`, `Guid` and `Enum` (`Enum.TryParse<T>`) has a `TryParse`. Use `Parse` only when bad input really is a bug.

## How do enums work in C#, including flags?

An `enum` is a named set of integer constants:

```csharp
public enum Stage { Basic, Stage1, Stage2 }     // 0, 1, 2
Stage s = Stage.Stage1;
int n = (int)s;                                   // 1
Stage parsed = Enum.Parse<Stage>("Basic");
```

For combinations, mark it `[Flags]` and use powers of two:

```csharp
[Flags]
public enum Condition { None = 0, Asleep = 1, Burned = 2, Confused = 4, Paralyzed = 8, Poisoned = 16 }

var c = Condition.Burned | Condition.Poisoned;
bool burned = (c & Condition.Burned) != 0;   // or c.HasFlag(Condition.Burned)
c &= ~Condition.Burned;                        // remove a flag
```

An enum variable can hold any integer value, even ones without a name, so validate input with `Enum.IsDefined` where it matters.

## What is the difference between a class and a struct in C#?

- A `class` is a reference type: it lives on the heap, is passed by reference, supports inheritance, and can be `null`.
- A `struct` is a value type: it's copied on assignment and when passed to methods, can't inherit from other structs or classes (it can implement interfaces), and can't be `null` unless it's nullable.

Use a struct for small, immutable values that behave like a single number: a point, a colour, a coordinate, an energy cost. Keep it small (around 16 bytes or less) and preferably `readonly struct`. Use a class for everything else, especially objects with identity or a lot of state.

A common struct bug is changing a copy instead of the original, e.g. `list[0].X = 5` doesn't compile for a `List<PointStruct>` because the indexer returns a copy.

## What are records in C#?

Records (C# 9+) are types designed for holding data. You get value-based equality, a readable `ToString()`, deconstruction and non-destructive copying for free:

```csharp
public record Card(string Name, int Hp);

var a = new Card("Pikachu", 60);
var b = new Card("Pikachu", 60);
Console.WriteLine(a == b);            // True: compared by value
var c = a with { Hp = 70 };           // a copy with one property changed
var (name, hp) = a;                   // deconstruct
```

`record` (or `record class`) is a reference type; `record struct` (C# 10) is a value type. The positional properties are `init`-only by default, so records are naturally immutable.

## What do `init` accessors do?

An `init` accessor (C# 9+) lets a property be set in an object initializer or constructor, but not afterwards. That gives immutable objects with the nice initializer syntax:

```csharp
public class Deck
{
    public string Name { get; init; } = "";
    public int Cards { get; init; }
}

var d = new Deck { Name = "Gardevoir", Cards = 60 };
d.Cards = 59;   // compile error: init-only property
```

Add `required` (C# 11) to force callers to set it: `public required string Name { get; init; }`.

## How do properties work in C#?

A property looks like a field to callers but runs code through `get` and `set` accessors:

```csharp
public class Pokemon
{
    private int damage;

    public int Hp { get; }                         // read-only auto-property (set in the constructor)
    public string Name { get; set; } = "";         // auto-property with a default
    public int Damage
    {
        get => damage;
        set => damage = Math.Max(0, value);        // validation
    }
    public int RemainingHp => Hp - Damage;         // computed, expression-bodied

    public Pokemon(int hp) { Hp = hp; }
}
```

Accessors can have their own access level: `public int Count { get; private set; }`. Properties should be cheap and have no surprising side effects; use a method for expensive or side-effecting work.

## What are the access modifiers in C#?

- `public`: accessible from anywhere.
- `private`: only inside the containing type (the default for class members).
- `protected`: the containing type and types derived from it.
- `internal`: anywhere in the same assembly (the default for top-level types).
- `protected internal`: same assembly, or derived types in other assemblies.
- `private protected` (C# 7.2): derived types, but only within the same assembly.

Start with the most restrictive access that works (`private`) and widen it only when another type needs it. That keeps the public surface small and makes refactoring safe.

## What does `static` mean in C#?

A `static` member belongs to the type itself rather than to an instance:

```csharp
public class Counter
{
    public static int Created;        // shared by all instances
    public int Id;
    public Counter() { Id = ++Created; }
    public static Counter Make() => new Counter();
}
```

A `static class` can't be instantiated and can only have static members; it suits helpers and extension methods (`Math`, `Enumerable`). A static constructor runs once, before the type is first used.

Be careful with static mutable state: it's shared across the whole program (and threads), which makes code harder to test and can cause race conditions.

## What's the difference between `ref`, `out` and `in` parameters?

All three pass a variable by reference instead of copying it:

- `ref`: the variable must be initialised before the call; the method can read and change it.
- `out`: the method must assign it before returning; the caller doesn't need to initialise it. Used for extra return values, as in `TryParse`.
- `in`: read-only by reference, which avoids copying a large struct without allowing changes.

```csharp
void Heal(ref int damage, int amount) => damage = Math.Max(0, damage - amount);
bool TryFind(string name, out Card card) { card = null; return false; }
double Length(in Vector3 v) => Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);

int dmg = 50; Heal(ref dmg, 30);   // dmg is now 20
```

For several return values, a tuple (`(bool ok, Card card)`) is often clearer than `out`.

## How do optional and named parameters work?

Optional parameters have a default value and can be left out; named arguments let you pass arguments by name, in any order, which makes calls with many options readable:

```csharp
void Train(int games, int threads = 4, bool gpu = false, double rest = 0.0) { }

Train(1000);                          // threads 4, gpu false
Train(1000, gpu: true);               // skip threads, set gpu
Train(games: 500, rest: 0.25, threads: 2);
```

Defaults must be compile-time constants (or `default`/`new()` for structs) and must come after the required parameters. Like `const`, default values are copied into the caller when it's compiled.

## What does the `params` keyword do?

`params` lets a method take a variable number of arguments as an array:

```csharp
int Sum(params int[] values) => values.Sum();

Sum();            // empty array
Sum(1, 2, 3);     // the compiler builds new[] { 1, 2, 3 }
Sum(numbers);     // or pass an existing array
```

It must be the last parameter, and there can be only one. Each call allocates an array, so avoid it in hot code paths (C# 13 also allows `params Span<T>`, which doesn't allocate).

## What are expression-bodied members?

A member whose body is a single expression can use `=>` instead of braces:

```csharp
public int RemainingHp => MaxHp - Damage;                   // read-only property
public override string ToString() => $"{Name} ({Hp} HP)";   // method
public void Log(string s) => Console.WriteLine(s);           // void method
public Card(string name) => Name = name;                     // constructor
```

They're purely a shorter way to write the same thing. Use them for simple one-liners; keep braces for anything with several steps.

## What are local functions in C#?

A local function is a method declared inside another method. It can use the enclosing method's variables and keeps a helper next to the only code that uses it:

```csharp
int CountEvolutions(Pokemon p)
{
    int count = 0;
    Visit(p);
    return count;

    void Visit(Pokemon mon)
    {
        count++;
        foreach (var e in mon.Evolutions) Visit(e);
    }
}
```

Unlike a lambda, a local function can be recursive and generic, and it doesn't allocate a delegate unless you convert it to one. Mark it `static` (C# 8+) if it shouldn't capture any variables.

## How do extension methods work?

An extension method adds a method to an existing type without changing or inheriting from it. It's a static method in a static class whose first parameter has `this`:

```csharp
public static class CardExtensions
{
    public static bool IsBasicEnergy(this CardDef card) =>
        card.Kind == CardKind.Energy && card.Basic;

    public static T PickRandom<T>(this IList<T> list, Random rng) => list[rng.Next(list.Count)];
}

if (card.IsBasicEnergy()) { }
var c = deck.PickRandom(rng);
```

The namespace containing the class must be imported with `using`. LINQ (`Where`, `Select`...) is built this way. Extension methods can't access private members, and a real instance method with the same signature always wins.

## How does method overloading work in C#?

Several methods can share a name if their parameter lists differ (number, types or order of parameters; `ref`/`out` also count). The compiler picks the best match at compile time:

```csharp
void Damage(PokemonInPlay target, int amount) { }
void Damage(PokemonInPlay target, int amount, bool applyWeakness) { }
void Damage(IEnumerable<PokemonInPlay> targets, int amount) { }
```

The return type alone isn't enough to overload. Overloads should do the same thing with different inputs; if they behave differently, give them different names. Optional parameters and overloads can conflict, so avoid mixing them heavily.

## How do inheritance, `virtual` and `override` work in C#?

A class can inherit from one base class. Members marked `virtual` (or `abstract`) can be replaced in a derived class with `override`, and the replacement is used even through a base-class reference (polymorphism):

```csharp
public class CardScript
{
    public virtual int ModifyDamage(int damage) => damage;
}

public class Exoskeleton : CardScript
{
    public override int ModifyDamage(int damage) => Math.Max(0, damage - 20);
}

CardScript s = new Exoskeleton();
s.ModifyDamage(60);   // 40: the override runs
```

Inside the override, `base.ModifyDamage(damage)` calls the base version. Without `virtual` a method can't be overridden; C# methods are non-virtual by default (unlike Java).

## What is the difference between `override` and `new` on a method?

- `override` replaces a `virtual` method: calls through a base-class reference run the derived version.
- `new` hides the base method: calls through a base-class reference still run the base version, and only calls through the derived type see the new one.

```csharp
class Base    { public virtual string A() => "base"; public string B() => "base"; }
class Derived : Base { public override string A() => "derived"; public new string B() => "derived"; }

Base x = new Derived();
x.A();  // "derived"
x.B();  // "base": hiding isn't polymorphic
```

Hiding is rarely what you want; it mainly exists to silence warning CS0108 when a base class later adds a member with the same name. Prefer `override`.

## What is the difference between an abstract class and an interface?

- An **abstract class** can have fields, constructors, implemented methods and abstract methods (which derived classes must implement). A class can inherit from only one class.
- An **interface** declares a contract: methods, properties, events and indexers, with no instance fields or constructors. A class can implement many interfaces. Since C# 8, interfaces can also have default method implementations.

```csharp
public interface IPlayerAgent { int Decide(DecisionRequest request); }

public abstract class Mon : CardScript
{
    protected virtual bool Attack(EffectContext ctx, AttackDef atk) => false;  // shared behaviour
    protected abstract string Name { get; }                                       // must be supplied
}
```

Use an interface for "can do" capabilities shared by unrelated types, and an abstract class for a family of related types sharing code and state.

## What does `sealed` do in C#?

- A `sealed class` can't be inherited from: `public sealed class Exoskeleton : Mon { }`.
- A `sealed override` method can't be overridden any further in derived classes.

Sealing leaf classes documents that they aren't meant to be extended, stops accidental inheritance, and lets the runtime make calls slightly faster (it can skip virtual dispatch). Many codebases seal classes by default and unseal only when there's a reason.

## How do constructors chain with `this(...)` and `base(...)`?

A constructor can call another constructor of the same class with `: this(...)`, or the base class's constructor with `: base(...)`. That keeps initialisation logic in one place:

```csharp
public class TrainingSession
{
    public int Games { get; }
    public int Seed { get; }

    public TrainingSession(int games) : this(games, Environment.TickCount) { }
    public TrainingSession(int games, int seed) { Games = games; Seed = seed; }
}

public class LeagueSession : TrainingSession
{
    public LeagueSession(int games) : base(games, 42) { }
}
```

The chained constructor runs first, then the body of the calling one. If a derived class doesn't call `base(...)`, the base class's parameterless constructor is used (and it's a compile error if there isn't one).

## What are object and collection initializers?

They set properties or add items right after construction, in one expression:

```csharp
var session = new TrainingSession(1000)
{
    Threads = 4,
    GpuMoves = true,
};

var names = new List<string> { "Pikachu", "Eevee" };
var hp = new Dictionary<string, int>
{
    ["Pikachu"] = 60,
    ["Snorlax"] = 150,
};
```

The constructor runs first, then each assignment in order. With C# 9 target-typed `new`, the type can be left out when it's known: `List<string> names = new() { "Pikachu" };`.

## How do I correctly override Equals and GetHashCode?

If two objects should count as equal by their contents, override both `Equals` and `GetHashCode` (and usually implement `IEquatable<T>`). Equal objects must return equal hash codes, otherwise `Dictionary` and `HashSet` break:

```csharp
public sealed class CardKey : IEquatable<CardKey>
{
    public string Set { get; }
    public string Number { get; }
    public CardKey(string set, string number) { Set = set; Number = number; }

    public bool Equals(CardKey other) => other != null && Set == other.Set && Number == other.Number;
    public override bool Equals(object obj) => Equals(obj as CardKey);
    public override int GetHashCode() => HashCode.Combine(Set, Number);
}
```

Only use fields that don't change while the object is in a dictionary or set. A `record` generates all of this for you.

## How do I sort objects in C# (IComparable, IComparer, OrderBy)?

Three common ways:

```csharp
// 1. LINQ: a new sorted sequence, the simplest for most cases
var byHp = cards.OrderByDescending(c => c.Hp).ThenBy(c => c.Name).ToList();

// 2. List.Sort with a comparison: sorts in place
cards.Sort((a, b) => a.Hp.CompareTo(b.Hp));

// 3. A natural order on the type itself
public class Card : IComparable<Card>
{
    public int Hp;
    public int CompareTo(Card other) => Hp.CompareTo(other.Hp);
}
```

A comparison returns a negative number if `a` comes first, zero if they're equal, and positive if `b` comes first. `List.Sort` is not stable (equal items may swap places); `OrderBy` is stable.

## What is operator overloading in C#?

A type can define what operators like `+`, `-`, `==` or `<` mean for it, with public static `operator` methods:

```csharp
public readonly struct Energy
{
    public readonly int Fire, Water;
    public Energy(int fire, int water) { Fire = fire; Water = water; }

    public static Energy operator +(Energy a, Energy b) => new Energy(a.Fire + b.Fire, a.Water + b.Water);
    public static bool operator ==(Energy a, Energy b) => a.Fire == b.Fire && a.Water == b.Water;
    public static bool operator !=(Energy a, Energy b) => !(a == b);
    public override bool Equals(object o) => o is Energy e && this == e;
    public override int GetHashCode() => HashCode.Combine(Fire, Water);
}
```

Some operators must come in pairs (`==` with `!=`, `<` with `>`), and if you overload `==` you should also override `Equals` and `GetHashCode`. Only overload operators when their meaning is obvious.

## What is an indexer in C#?

An indexer lets instances of your class be used with square brackets like an array:

```csharp
public class Bench
{
    private readonly PokemonInPlay[] slots = new PokemonInPlay[5];

    public PokemonInPlay this[int index]
    {
        get => slots[index];
        set => slots[index] = value;
    }
}

bench[0] = pikachu;
var first = bench[0];
```

An indexer can take any parameter types (e.g. `this[string name]`) and can be overloaded.

## How do generics work in C#?

Generics let you write a class or method once and use it with any type, while keeping full type safety and avoiding casts:

```csharp
public class Pool<T>
{
    private readonly List<T> items = new();
    public void Add(T item) => items.Add(item);
    public T Pick(Random rng) => items[rng.Next(items.Count)];
}

var decks = new Pool<Deck>();
static T Max<T>(T a, T b) where T : IComparable<T> => a.CompareTo(b) >= 0 ? a : b;
```

Unlike Java, .NET keeps the real type at runtime: `List<int>` stores actual `int`s without boxing, and `typeof(T)` works inside generic code.

## What are generic constraints (`where T : ...`)?

Constraints limit which types can be used for a type parameter, and in return let you use those types' members:

```csharp
T Clone<T>(T x) where T : ICloneable => (T)x.Clone();
T Make<T>() where T : new() => new T();
void Save<T>(T x) where T : class { }          // reference types only
void Pack<T>(T x) where T : struct { }         // non-nullable value types only
void Show<T>(T x) where T : CardScript { }     // CardScript or a subclass
void Fill<T>(Span<T> s) where T : unmanaged { } // plain value types with no references
```

Several constraints can be combined: `where T : class, IComparable<T>, new()`.

## What is covariance and contravariance in C# generics?

Variance controls whether a generic type of a derived type can be used where one of a base type is expected.

- **Covariance (`out T`)**: a producer of `Dog` can be used as a producer of `Animal`. `IEnumerable<out T>` is covariant, so an `IEnumerable<string>` can be passed where an `IEnumerable<object>` is wanted.
- **Contravariance (`in T`)**: a consumer of `Animal` can be used as a consumer of `Dog`. `Action<in T>` and `IComparer<in T>` are contravariant.

```csharp
IEnumerable<object> things = new List<string> { "a" };   // OK: covariance
Action<object> logAny = o => Console.WriteLine(o);
Action<string> logString = logAny;                         // OK: contravariance
```

Only interfaces and delegates can be variant, not classes: `List<string>` can't be used as `List<object>`, because the list could then accept a non-string.

## What is boxing and why does it matter for performance?

Boxing happens when a value type is converted to `object` (or to an interface it implements): the runtime allocates a heap object and copies the value into it. Unboxing copies it back out.

```csharp
int n = 42;
object o = n;           // boxing: a heap allocation
int m = (int)o;         // unboxing
ArrayList old = new();  // the old non-generic collections box every int
```

Each box is a small allocation that the garbage collector later has to clean up. In hot code (game loops, simulations) avoid: non-generic collections, `string.Format`/interpolation with value types in older .NET versions, calling interface methods on structs through the interface type, and `enum.HasFlag` on old runtimes. Generics (`List<int>`, `Dictionary<int, T>`) avoid boxing entirely.

## Which collection should I use in C#: List, Dictionary, HashSet, Queue or Stack?

- `List<T>`: ordered, indexable, fast to add at the end. The default choice.
- `Dictionary<TKey, TValue>`: fast lookup by key (roughly constant time).
- `HashSet<T>`: unique items with fast "contains" checks; also set operations (`UnionWith`, `IntersectWith`).
- `Queue<T>`: first in, first out (a waiting line, breadth-first search).
- `Stack<T>`: last in, first out (undo, depth-first search).
- `SortedDictionary`/`SortedSet`: kept in key order (slower than the hash-based ones).
- `LinkedList<T>`: cheap inserts and removals in the middle when you already have the node; rarely the best choice.

Rule of thumb: if you call `list.Contains(x)` or search a list repeatedly, you probably want a `HashSet` or `Dictionary`.

## What's the difference between an array and a List<T>?

- An **array** (`int[]`) has a fixed length set when it's created. It's the most compact and fastest storage, and it supports multiple dimensions (`int[,]`).
- A **`List<T>`** wraps an array that grows automatically (it doubles its capacity when full). It adds `Add`, `Remove`, `Insert`, `Contains`, `Sort` and so on.

Use an array when the size is known and fixed (a board, a buffer, a lookup table), and a `List<T>` when items come and go. If you know roughly how many items a list will hold, pass the capacity (`new List<Card>(60)`) to avoid repeated growing.

## How do I safely get a value from a Dictionary?

Indexing with a missing key throws `KeyNotFoundException`. Use `TryGetValue`:

```csharp
if (hpByName.TryGetValue("Pikachu", out int hp))
    Console.WriteLine(hp);

int hp2 = hpByName.GetValueOrDefault("Mew", 0);   // .NET Core 2.0+ / via CollectionExtensions
```

To add or update: `dict[key] = value` (adds or overwrites). `dict.Add(key, value)` throws if the key already exists; `dict.TryAdd(key, value)` doesn't. To count things:

```csharp
counts[name] = counts.TryGetValue(name, out int c) ? c + 1 : 1;
```

## Why do I get 'Collection was modified; enumeration operation may not execute'?

You changed a collection (added, removed or replaced items) while a `foreach` was looping over it. The enumerator notices and throws `InvalidOperationException`.

Fixes:

```csharp
// 1. Loop over a copy
foreach (var mon in bench.ToList())
    if (mon.IsKnockedOut) bench.Remove(mon);

// 2. Use RemoveAll for lists
bench.RemoveAll(m => m.IsKnockedOut);

// 3. Loop backwards by index
for (int i = bench.Count - 1; i >= 0; i--)
    if (bench[i].IsKnockedOut) bench.RemoveAt(i);

// 4. Collect first, change after
var toRemove = dict.Where(kv => kv.Value == 0).Select(kv => kv.Key).ToList();
foreach (var k in toRemove) dict.Remove(k);
```

## What are Span<T> and Memory<T>?

`Span<T>` is a lightweight view over a contiguous block of memory: part of an array, a string, stack memory or native memory. Slicing it doesn't copy or allocate:

```csharp
int[] data = { 1, 2, 3, 4, 5 };
Span<int> middle = data.AsSpan(1, 3);   // 2, 3, 4: no copy
middle[0] = 20;                          // changes data[1]

ReadOnlySpan<char> word = "Pikachu".AsSpan(0, 4);   // "Pika" without a new string
Span<byte> buffer = stackalloc byte[256];           // stack memory, no GC
```

`Span<T>` is a `ref struct`: it can only live on the stack, so it can't be a field of a class, be boxed, or be used across `await`. `Memory<T>` is the heap-friendly version for those cases. They're mainly for performance-critical parsing and buffer code.

## What's the difference between IEnumerable, ICollection, IList and IReadOnlyList?

They're interfaces with increasing abilities:

- `IEnumerable<T>`: can be looped over with `foreach`. Nothing else (no count, no index). It might be lazy, or even infinite.
- `ICollection<T>`: adds `Count`, `Add`, `Remove`, `Contains`.
- `IList<T>`: adds indexing (`list[i]`) and `Insert`/`RemoveAt`.
- `IReadOnlyCollection<T>` / `IReadOnlyList<T>`: `Count` and indexing without any way to change the collection.

For parameters, ask for the least you need (`IEnumerable<T>` if you only loop once, `IReadOnlyList<T>` if you need a count or index). For return values, `IReadOnlyList<T>` tells callers they shouldn't modify the result.

## What does `yield return` do?

`yield return` turns a method into an iterator: it produces values one at a time, only as the caller asks for them, without building a whole list first:

```csharp
IEnumerable<int> Fibonacci()
{
    int a = 0, b = 1;
    while (true)
    {
        yield return a;
        (a, b) = (b, a + b);
    }
}

foreach (var f in Fibonacci().Take(10)) Console.WriteLine(f);
```

`yield break` ends the sequence early. The method's code doesn't run until enumeration starts, and it runs again from the start each time the result is enumerated, so argument validation inside it happens late (put it in a separate non-iterator wrapper if it matters).

## What are delegates, Func and Action in C#?

A delegate is a type-safe reference to a method: a variable that holds "something you can call".

- `Action<T1, T2...>`: takes arguments and returns nothing.
- `Func<T1, T2..., TResult>`: takes arguments and returns a value (the last type parameter is the return type).
- `Predicate<T>`: takes a `T`, returns `bool` (`Func<T, bool>` is more common today).

```csharp
Func<int, int, int> add = (a, b) => a + b;
Action<string> log = Console.WriteLine;
Func<CardDef, bool> isBasic = d => d.IsBasicPokemon;

add(2, 3);                          // 5
var basics = cards.Where(isBasic);  // pass behaviour as a value
```

Delegates are how C# passes behaviour around: callbacks, LINQ, event handlers and strategy-style code.

## How do lambdas capture variables, and what's the loop-variable trap?

A lambda can use local variables from the method around it. It captures the **variable**, not a copy of its value, so later changes are visible inside the lambda:

```csharp
int bonus = 10;
Func<int, int> addBonus = x => x + bonus;
bonus = 50;
addBonus(1);   // 51, not 11
```

The classic trap is capturing a `for` loop variable, which is one variable shared by every iteration:

```csharp
var actions = new List<Action>();
for (int i = 0; i < 3; i++) actions.Add(() => Console.WriteLine(i));
foreach (var a in actions) a();   // prints 3, 3, 3

for (int i = 0; i < 3; i++) { int copy = i; actions.Add(() => Console.WriteLine(copy)); }  // 0, 1, 2
```

`foreach` gets a fresh variable each iteration (since C# 5), so it doesn't have this problem. Captured variables also live as long as the lambda does, which can keep objects alive longer than expected.

## How do events work in C#?

An event is a delegate field that outside code can only subscribe to (`+=`) or unsubscribe from (`-=`). Only the owning class can raise it:

```csharp
public class GameEngine
{
    public event Action<GameEvent> EventRaised;

    void Emit(GameEvent e) => EventRaised?.Invoke(e);   // ?. skips it when nobody listens
}

engine.EventRaised += e => Console.WriteLine(e.Text);
```

The .NET convention is `event EventHandler<TArgs>` with `(object sender, TArgs e)`. Always unsubscribe long-lived subscribers: an event keeps every subscriber alive, a common source of memory leaks (especially in Unity, where destroyed objects can still receive callbacks).

## What is LINQ and what are the most useful LINQ methods?

LINQ (Language Integrated Query) is a set of extension methods for querying any `IEnumerable<T>`:

```csharp
using System.Linq;

var strong = cards.Where(c => c.Hp >= 120)          // filter
                  .OrderByDescending(c => c.Hp)    // sort
                  .Select(c => c.Name)             // transform
                  .ToList();                        // run it and store the results

int total = deck.Sum(c => c.Count);
bool anyEx = deck.Any(c => c.IsEx);
bool allBasic = bench.All(m => m.IsBasic);
var byType = cards.GroupBy(c => c.Type).ToDictionary(g => g.Key, g => g.Count());
var top3 = scores.OrderByDescending(s => s).Take(3);
var unique = names.Distinct();
var first = cards.FirstOrDefault(c => c.Name == "Mew");
```

Others worth knowing: `SelectMany` (flatten), `Count`, `Min`/`Max`/`MaxBy`, `Skip`, `Zip`, `Concat`, `Except`, `Intersect`, `ToHashSet`, `Aggregate`.

## What is deferred execution in LINQ?

Most LINQ methods (`Where`, `Select`, `OrderBy`...) don't run when you call them. They build a query that runs when you enumerate it (`foreach`, `ToList()`, `Count()`, `First()`...), and it runs again **every time** you enumerate it:

```csharp
var alive = bench.Where(m => m.Hp > 0);   // nothing has run yet
bench.Add(newMon);
alive.Count();                            // runs now, sees newMon
alive.Count();                            // runs the filter again
```

Problems this causes: repeated expensive work, results changing because the source changed, and exceptions appearing far from the line that built the query. Call `ToList()` or `ToArray()` when you want a snapshot or will enumerate the result more than once.

## What is the difference between First, FirstOrDefault, Single and SingleOrDefault?

- `First()`: the first matching item; throws if there are none.
- `FirstOrDefault()`: the first matching item, or `default` (`null` for classes, `0` for `int`) if none.
- `Single()`: the only matching item; throws if there are none **or more than one**.
- `SingleOrDefault()`: the only item or `default`; still throws if there's more than one.

```csharp
var mew = cards.First(c => c.Name == "Mew");            // must exist
var maybe = cards.FirstOrDefault(c => c.Name == "Mew"); // may be null
var key = cards.Single(c => c.Key == "LM-88");          // must be unique
```

Use `Single` when "more than one" would be a bug you want to catch. `First` stops at the first match, so it's also faster than `Single`, which has to keep looking.

## When should I avoid LINQ for performance?

LINQ is clear and usually fast enough, but in hot code (called thousands of times per frame or per simulated game) it costs:

- an allocation for each iterator and for each lambda that captures variables,
- delegate calls per item,
- hidden repeated work if a deferred query is enumerated several times.

In those spots a plain `for`/`foreach` loop is faster and allocation-free:

```csharp
// LINQ
int energy = mon.Energy.Count(e => e.Def.IsBasicEnergy);

// Hot-path version
int energy2 = 0;
for (int i = 0; i < mon.Energy.Count; i++)
    if (mon.Energy[i].Def.IsBasicEnergy) energy2++;
```

Measure first (a profiler or `Stopwatch`); rewrite only the loops that show up.

## How does pattern matching work in C#?

Pattern matching tests a value's shape and extracts parts of it in one step:

```csharp
// Type pattern with a variable
if (card.Script is Mon mon) mon.Attack(ctx);

// Property pattern
if (pokemon is { Hp: > 100, IsEx: true }) { }

// Relational and logical patterns (C# 9)
string Size(int hp) => hp switch
{
    < 60 => "small",
    >= 60 and < 120 => "medium",
    _ => "large",
};

// null checks
if (target is null) return;
if (target is not null) { }
```

The `is` checks are also safe: they never throw, and `is null` can't be fooled by an overloaded `==`.

## What are switch expressions?

A switch expression (C# 8) picks a value based on patterns, more compactly than a `switch` statement:

```csharp
string Describe(Stage stage) => stage switch
{
    Stage.Basic => "Basic Pokémon",
    Stage.Stage1 => "Stage 1",
    Stage.Stage2 => "Stage 2",
    _ => throw new ArgumentOutOfRangeException(nameof(stage)),
};

int Prizes(PokemonInPlay mon) => mon switch
{
    { IsVmax: true } => 3,
    { IsEx: true } or { IsV: true } => 2,
    _ => 1,
};
```

Arms are checked top to bottom, `_` is the catch-all, and the compiler warns if some inputs aren't handled.

## How do tuples and deconstruction work in C#?

Value tuples group a few values without declaring a type:

```csharp
(string Name, int Hp) Best(List<Card> cards)
{
    var best = cards.MaxBy(c => c.Hp);
    return (best.Name, best.Hp);
}

var (name, hp) = Best(deck);         // deconstruct into variables
var result = Best(deck);
Console.WriteLine(result.Name);       // named elements
(a, b) = (b, a);                      // swap without a temp variable
```

They're value types (no allocation) and compare by value. Use them for private helpers and multiple return values; for anything public or long-lived, a named `record` or class reads better. Any type can support deconstruction by adding a `Deconstruct(out ...)` method.

## What are good practices for exceptions in C#?

- Throw exceptions for exceptional situations (bugs, broken invariants, failed I/O), not for normal control flow; use `TryParse`/`TryGetValue`-style methods for expected failures.
- Catch only exceptions you can handle, as specifically as possible (`catch (IOException e)`), not a blanket `catch (Exception)` that hides bugs. A top-level catch-all that logs is fine.
- Rethrow with `throw;` (keeps the original stack trace), not `throw e;`.
- Use `finally` or `using` for cleanup that must always happen.
- Validate arguments early: `ArgumentNullException.ThrowIfNull(card);`, `throw new ArgumentOutOfRangeException(nameof(count))`.
- Exception filters catch conditionally: `catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)`.

Throwing is slow (it captures a stack trace), so never throw in a tight loop as a way to signal ordinary results.

## What does `using` do with IDisposable?

Objects that hold unmanaged resources (files, sockets, database connections, GPU buffers) implement `IDisposable`. `using` guarantees `Dispose()` is called when the block ends, even if an exception is thrown:

```csharp
using (var reader = new StreamReader(path))
{
    string text = reader.ReadToEnd();
}   // reader.Dispose() runs here

// C# 8 using declaration: disposed at the end of the enclosing scope
using var client = new TcpClient();
```

It compiles to a `try/finally`. Not disposing usually means the resource stays open until the garbage collector finalises the object, which may be much later (locked files, exhausted sockets). For `async` resources, use `await using` with `IAsyncDisposable`.

## How does garbage collection work in .NET?

The garbage collector (GC) frees objects that nothing references any more, so you don't free memory yourself. The .NET GC is generational:

- New objects start in **generation 0**, which is collected often and cheaply.
- Objects that survive are promoted to gen 1, then gen 2 (collected rarely and more expensively).
- Objects of 85,000 bytes or more go on the **large object heap**, collected with gen 2.

Practical consequences: short-lived small allocations are cheap but not free; many of them in a hot loop cause frequent collections (in Unity, visible as frame hitches). Long-lived caches belong in gen 2. Reduce allocations in hot paths with object pooling, reused buffers, `struct`s, `Span<T>` and `StringBuilder`. A memory "leak" in .NET usually means something still references the object: a static list, an event subscription or a cache.

## What is a finalizer and when do I need one?

A finalizer (`~MyClass()`) runs when the garbage collector reclaims an object. You almost never need one: it's only for classes that directly own an unmanaged handle (a raw OS handle or native memory) not already wrapped in a `SafeHandle`.

Finalizers make objects more expensive (they survive an extra GC), run at an unpredictable time on a separate thread, and may not run at all at shutdown. The usual pattern is to implement `IDisposable` for deterministic cleanup, and call `GC.SuppressFinalize(this)` in `Dispose()` if a finalizer exists as a safety net.

## How do async and await work in C#?

`async`/`await` lets a method wait for slow work (network, disk, a timer) without blocking a thread. `await` pauses the method, frees the thread, and continues the method when the awaited `Task` finishes:

```csharp
async Task<string> LoadDeckAsync(string path)
{
    string text = await File.ReadAllTextAsync(path);   // the thread is free while the disk works
    return text.Trim();
}

string deck = await LoadDeckAsync("deck.txt");
```

- An async method returns `Task` (no result), `Task<T>` (a result), or `ValueTask<T>` (to avoid allocation when it often finishes synchronously).
- `async` doesn't create threads: async I/O uses no thread at all while waiting. For CPU-heavy work, use `await Task.Run(() => Compute())` to move it off the UI thread.
- Exceptions in the async method are thrown at the `await`.

## Why should I avoid `async void`?

An `async void` method can't be awaited, so callers don't know when it finishes, and an exception thrown inside it can't be caught by the caller: it goes straight to the synchronization context and usually crashes the app.

```csharp
async void Bad() { await Task.Delay(10); throw new Exception(); }   // can't be caught
async Task Good() { await Task.Delay(10); throw new Exception(); }  // caller can await and catch
```

The only legitimate use is event handlers whose signature must return `void` (e.g. a UI button click). Even there, wrap the body in a try/catch.

## Why does calling .Result or .Wait() on a Task cause deadlocks?

`.Result` and `.Wait()` block the current thread until the task finishes. If the task needs to resume on that same thread (UI apps, older ASP.NET, Unity's main thread), it can't, because that thread is blocked waiting for it: a deadlock.

```csharp
// On a UI thread: deadlock
var text = LoadDeckAsync(path).Result;

// Fix: await all the way up
var text2 = await LoadDeckAsync(path);
```

The rule is "async all the way": once something is async, make its callers async too. In library code, `await task.ConfigureAwait(false)` avoids resuming on the original context, which removes that deadlock and is slightly faster. Only block on tasks in console apps' `Main` or tests, and prefer `async Task Main` there too.

## How do I run several tasks at once and wait for all of them?

Start them all first, then await `Task.WhenAll`:

```csharp
var downloads = urls.Select(u => http.GetStringAsync(u)).ToList();   // all start now
string[] pages = await Task.WhenAll(downloads);

// First one to finish
var winner = await Task.WhenAny(taskA, taskB);
```

Awaiting each task in a loop (`foreach (var u in urls) await http.GetStringAsync(u);`) runs them one after another. To limit how many run at once, use `Parallel.ForEachAsync` (.NET 6+) with `MaxDegreeOfParallelism`, or a `SemaphoreSlim`.

## How do cancellation tokens work?

A `CancellationToken` lets a caller ask long-running work to stop. The work checks the token and stops cleanly:

```csharp
var cts = new CancellationTokenSource();
cts.CancelAfter(TimeSpan.FromMinutes(3));          // or cts.Cancel() from a Stop button

async Task TrainAsync(int games, CancellationToken ct)
{
    for (int g = 0; g < games; g++)
    {
        ct.ThrowIfCancellationRequested();         // throws OperationCanceledException
        await PlayGameAsync(ct);                   // pass it down to anything that accepts one
    }
}
```

Cancellation is cooperative: nothing stops unless the code checks the token. Work that is blocked in a call that doesn't take a token (a socket read, for example) won't notice it, so close or abort that resource instead (or set a timeout). Dispose the `CancellationTokenSource` when you're done.

## How do I protect shared data between threads in C#?

Whenever two threads touch the same data and at least one writes, you need synchronization:

```csharp
private readonly object gate = new object();
private int done;

void GameFinished()
{
    lock (gate)              // only one thread at a time runs this block
    {
        done++;
        results.Add(lastResult);
    }
}

// A single counter: lock-free and faster
Interlocked.Increment(ref done);
```

Guidelines: lock on a private object (never `this` or a `Type`), keep locked sections short, never `await` inside a `lock`, and always take several locks in the same order to avoid deadlocks. For shared collections, `ConcurrentDictionary`, `ConcurrentQueue` and `ConcurrentBag` handle the locking for you.

## What do `volatile` and `Interlocked` do?

- `Interlocked` makes single operations atomic across threads: `Interlocked.Increment(ref n)`, `Interlocked.Add`, `Interlocked.Exchange`, `Interlocked.CompareExchange`. Without it, `n++` from two threads can lose updates, because it's a read, an add and a write.
- `volatile` (or `Volatile.Read`/`Volatile.Write`) makes sure a read sees the latest written value and stops the compiler and CPU reordering around it. It does **not** make `n++` atomic.

```csharp
private volatile bool cancel;        // a flag one thread sets and others poll
public void Cancel() => cancel = true;

private int played;
int g = Interlocked.Increment(ref played) - 1;   // safe unique index per finished game
```

Use `volatile` for simple flags, `Interlocked` for counters, and `lock` for anything that changes several values together.

## What does [ThreadStatic] do in C#?

`[ThreadStatic]` on a static field gives each thread its own separate copy of it:

```csharp
[ThreadStatic] private static int copyDepth;   // each thread counts its own recursion

void UseCopiedAttack()
{
    if (copyDepth >= 3) return;                 // guard against infinite copying
    copyDepth++;
    try { ResolveAttack(); }
    finally { copyDepth--; }
}
```

That's useful for per-thread state such as recursion guards, scratch buffers or random number generators in code run by many threads at once. Caveats: don't give it an initializer (`= 5` only runs on the first thread; other threads see 0), and with `async` code the method may continue on a different thread. `ThreadLocal<T>` supports initialisation; `AsyncLocal<T>` follows the async flow instead of the thread.

## Is System.Random thread-safe?

No. `Random` isn't thread-safe: using one instance from several threads at once can corrupt its state, after which it may return only zeros.

Options:
- .NET 6+: `Random.Shared` is a thread-safe instance to share.
- Give each thread or each worker its own `Random`, for example seeded from a master seed plus the worker's index (this also makes multi-threaded simulations reproducible).
- `[ThreadStatic] static Random rng;` created lazily on each thread.

For security-sensitive randomness (tokens, keys), use `System.Security.Cryptography.RandomNumberGenerator` instead. Note also that `new Random(seed)` gives the same sequence for the same seed, which is useful for reproducible games and tests.

## What's the difference between Task.Run, Parallel.For and Thread?

- `new Thread(...)`: a dedicated OS thread. Expensive to create; use it for long-running loops that need their own thread (and set `IsBackground = true` so it doesn't keep the process alive).
- `Task.Run(...)`: queues work to the thread pool and returns a `Task` you can await. The default for "run this off the main thread".
- `Parallel.For` / `Parallel.ForEach`: splits a loop across thread-pool threads and blocks until all iterations finish. Good for CPU-bound data processing:

```csharp
Parallel.For(0, games, new ParallelOptions { MaxDegreeOfParallelism = 8 }, i =>
{
    var result = PlayGame(seedBase + i);
    lock (gate) results.Add(result);
});
```

Don't call `Parallel.For` inside other parallel code without limits, and don't use it for I/O-bound work (use async instead).

## How do I read and write files in C#?

For whole files, the `File` helpers are simplest:

```csharp
string text = File.ReadAllText(path);
string[] lines = File.ReadAllLines(path);
File.WriteAllText(path, text, new UTF8Encoding(false));   // UTF-8 without a BOM
File.AppendAllText(logPath, line + Environment.NewLine);

// Large files: stream line by line
foreach (var line in File.ReadLines(path)) { }

// Paths: never glue them with "+" and "\\"
string full = Path.Combine(folder, "decks", name + ".txt");
Directory.CreateDirectory(Path.GetDirectoryName(full));
```

To write safely (so a crash can't leave a half-written file), write to a temporary file and then replace the original with `File.Move(tmp, path, overwrite: true)` or `File.Replace`.

## How do I serialize and deserialize JSON in C#?

With `System.Text.Json` (built into .NET Core 3+):

```csharp
using System.Text.Json;

public class Card { public string Name { get; set; } public int Hp { get; set; } }

string json = JsonSerializer.Serialize(card, new JsonSerializerOptions { WriteIndented = true });
Card back = JsonSerializer.Deserialize<Card>(json);

var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
List<Card> cards = JsonSerializer.Deserialize<List<Card>>(File.ReadAllText(path), options);
```

It works on public properties by default (add `IncludeFields = true` for fields). Newtonsoft.Json (`JsonConvert.SerializeObject`) is the older, more flexible library. In Unity, `JsonUtility` is built in but limited: no dictionaries, it serializes public fields or `[SerializeField]` ones, and the top level must be an object, not an array.

## What's the difference between DateTime, DateTimeOffset and Stopwatch?

- `DateTime`: a date and time, with a `Kind` (Local, Utc or Unspecified) that's easy to mix up. Use `DateTime.UtcNow` for timestamps you store.
- `DateTimeOffset`: a date and time plus its offset from UTC, so it's unambiguous. Prefer it for anything saved or compared across machines.
- `Stopwatch`: a high-resolution timer for measuring how long code takes. `DateTime.Now` is too coarse and can jump (clock changes):

```csharp
var sw = Stopwatch.StartNew();
RunGames(1000);
Console.WriteLine($"{sw.Elapsed.TotalSeconds:F1}s, {1000 / sw.Elapsed.TotalSeconds:F1} games/s");
```

For durations, use `TimeSpan` (`TimeSpan.FromMinutes(3)`, `(end - start).TotalSeconds`).

## What are attributes in C#?

Attributes attach metadata to code that compilers, frameworks or your own code can read:

```csharp
[Serializable] public class SaveData { }
[Obsolete("Use PlayTurn instead")] public void OldTurn() { }
[Flags] public enum Condition { }
[Test] public void Burn_Puts_Two_Counters() { }           // NUnit
[SerializeField] private int maxHp;                        // Unity: show a private field in the Inspector
[ThreadStatic] private static int depth;
```

You can define your own by subclassing `Attribute`, and read them with reflection (`typeof(T).GetCustomAttributes(...)`). Attributes don't change behaviour by themselves; something has to look for them.

## What is reflection in C# and when should I use it?

Reflection inspects and uses types at runtime: listing a class's methods, reading attributes, creating instances by name, or calling members you only know at runtime:

```csharp
Type t = typeof(CardScript);
bool overrides = script.GetType().GetMethod("StopsPokeBodiesOf").DeclaringType != t;
var instance = (CardScript)Activator.CreateInstance(Type.GetType("PokemonTCG.Cards.MukStench"));
foreach (var p in typeof(Card).GetProperties()) Console.WriteLine(p.Name);
```

It's powerful (plugins, serializers, test frameworks) but slow compared with normal calls, not checked by the compiler, and fragile when code is renamed or trimmed (IL2CPP in Unity can strip unused members). Cache reflection results instead of repeating lookups in hot code, and prefer interfaces or virtual methods when you can.

## How do I write unit tests in C# with NUnit?

A test class has methods marked `[Test]`, each arranging a situation, acting, and asserting the result:

```csharp
using NUnit.Framework;

public class DamageTests
{
    private GameEngine engine;

    [SetUp]
    public void SetUp() => engine = new GameEngine(seed: 42);

    [Test]
    public void Weakness_Doubles_Damage()
    {
        var target = engine.PutActive(water: true, weakness: EnergyType.Lightning);
        engine.Attack(lightningAttacker, damage: 30);
        Assert.AreEqual(60, target.Damage);
    }

    [TestCase(1, 10)]
    [TestCase(3, 30)]
    public void Damage_Scales_With_Counters(int counters, int expected) =>
        Assert.AreEqual(expected, counters * 10);
}
```

Good tests are small, independent, deterministic (fixed random seeds) and named after the behaviour they check. Run them with `dotnet test` or in your IDE's test runner (in Unity, the Test Runner window for EditMode/PlayMode tests).

## What is dependency injection in C#?

Dependency injection means a class receives the things it depends on (usually as interfaces) through its constructor instead of creating them itself:

```csharp
public interface IPlayerAgent { int Decide(DecisionRequest r); }

public class GameEngine
{
    private readonly IPlayerAgent player1, player2;
    public GameEngine(IPlayerAgent p1, IPlayerAgent p2) { player1 = p1; player2 = p2; }
}

var engine = new GameEngine(new NeuralAgent(net), new ScriptedAgent());   // or a test double
```

That makes code easy to test (pass a fake or scripted dependency) and to reconfigure. ASP.NET Core and other hosts have a built-in container (`services.AddSingleton<IService, Service>()`) that builds these object graphs for you, but plain constructor parameters are already dependency injection.

## What is the order of Unity's MonoBehaviour event methods?

The main ones, in order:

1. `Awake()`: when the object is created (even if the script is disabled). Set up references to its own components here.
2. `OnEnable()`: each time the object or script becomes enabled.
3. `Start()`: once, before the first `Update`, only if enabled. Use it for setup that needs other objects' `Awake` to have run.
4. `FixedUpdate()`: on a fixed timestep (default 50 times a second), for physics.
5. `Update()`: once per frame, for input and game logic.
6. `LateUpdate()`: after all `Update`s, e.g. for a camera following a moved object.
7. `OnGUI()`: for immediate-mode (IMGUI) UI, possibly several times per frame.
8. `OnDisable()` and `OnDestroy()`: when disabled or destroyed.

Don't use constructors in MonoBehaviours; Unity creates them itself, so put initialisation in `Awake`/`Start`.

## How do coroutines work in Unity?

A coroutine is an `IEnumerator` method that Unity runs across several frames, pausing at each `yield`:

```csharp
IEnumerator FlashCard(Renderer r)
{
    for (int i = 0; i < 3; i++)
    {
        r.enabled = false;
        yield return new WaitForSeconds(0.1f);
        r.enabled = true;
        yield return new WaitForSeconds(0.1f);
    }
}

StartCoroutine(FlashCard(cardRenderer));
```

`yield return null` waits one frame; `WaitForSeconds`, `WaitUntil(() => done)` and `WaitForEndOfFrame` wait for other things. Coroutines run on the main thread (they aren't threads) and stop when their GameObject is deactivated or destroyed. Unity 2023.1+ also has `Awaitable` for `async`/`await` that stays on the main thread.

## Why is `== null` special for Unity objects?

Unity overrides `==` for `UnityEngine.Object` (GameObjects, components, assets). After `Destroy(obj)`, the C# object still exists, but `obj == null` returns `true` because the native object behind it is gone.

The C# null-conditional and null-coalescing operators (`?.`, `??`, `is null`) don't use that override, so they treat a destroyed object as **not** null:

```csharp
Destroy(enemy);
// next frame:
if (enemy == null) { }        // true: Unity's check
enemy?.TakeDamage(1);         // still calls it: MissingReferenceException
```

For Unity objects, use `== null`, `!= null` or `if (enemy)` rather than `?.`, `??` or `is null`.

## Why should I cache GetComponent calls in Unity?

`GetComponent<T>()`, `FindObjectOfType` and `GameObject.Find` search at runtime. Calling them every frame in `Update` wastes time, and the `Find` methods are slow. Look them up once and keep the reference:

```csharp
public class CardView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer art;   // assign in the Inspector, or:
    private Rigidbody body;

    void Awake() => body = GetComponent<Rigidbody>();

    void Update()
    {
        body.AddForce(Vector3.up);   // no lookup every frame
    }
}
```

Likewise, cache `Camera.main` in older Unity versions, and avoid `new` allocations, LINQ and string building inside `Update`.

## What does [SerializeField] do in Unity?

Unity saves and shows in the Inspector only fields that it serializes: public fields and non-public fields marked `[SerializeField]`. That lets you keep a field private in code but still edit it in the Editor:

```csharp
public class Deck : MonoBehaviour
{
    [SerializeField] private int maxCards = 60;
    [SerializeField] private List<CardData> cards = new();
    [HideInInspector] public int shuffles;       // public but not shown
    [NonSerialized] public int cache;            // public but not saved
}
```

Unity doesn't serialize properties (unless you use `[field: SerializeField]` on an auto-property), static fields, dictionaries, or most interfaces; use a list of key/value structs instead of a dictionary. A value set in the Inspector overrides the field's initializer in code.

## What are ScriptableObjects in Unity?

A ScriptableObject is a data container saved as an asset rather than attached to a GameObject. It suits shared, read-mostly data like card definitions, settings or level data:

```csharp
[CreateAssetMenu(menuName = "TCG/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int hp;
    public Sprite art;
}
```

Create instances from the Assets → Create menu and reference them from MonoBehaviours. Many objects can share one asset without duplicating the data. Changes made to a ScriptableObject in Play Mode in the Editor stay in the asset afterwards, but in a built game they don't persist between runs.

## Can I use threads and async in Unity?

Yes, with one rule: the Unity API (GameObjects, components, `Transform`, most of `UnityEngine`) can only be used from the main thread.

- Pure C# work (simulations, AI search, file parsing) can run on other threads with `Task.Run`, `Thread` or `Parallel.For`, then hand the result back to the main thread, for example through a field checked in `Update` or a `ConcurrentQueue` of actions.
- `async`/`await` started on the main thread resumes on the main thread (Unity has its own synchronization context), so `await File.ReadAllTextAsync(...)` followed by Unity calls is fine.
- For heavy number crunching, the Job System with Burst compiles parallel code to very fast native code.
- Background tasks don't stop when you leave Play Mode in the Editor; pass a `CancellationToken` and cancel it in `OnDestroy`/`OnDisable`.

Long work can also run in a separate process (a command-line program), which keeps the editor responsive.

## How do I keep garbage-collection spikes down in Unity?

Unity's garbage collector can pause the game when it runs, so avoid allocating memory every frame:

- Don't create `new` lists, arrays or strings in `Update`; reuse fields and `Clear()` them.
- Avoid LINQ and lambdas that capture variables in per-frame code.
- Avoid string concatenation for UI every frame; update text only when the value changes.
- Use object pooling for things spawned often (bullets, cards, particles): deactivate and reuse instead of `Destroy`/`Instantiate`. `UnityEngine.Pool.ObjectPool<T>` exists for this.
- Prefer the non-allocating APIs: `Physics.RaycastNonAlloc`, `GetComponents(list)`.
- Enable "Incremental GC" in Player Settings to spread collections over frames.

Use the Profiler's "GC Alloc" column to find what's allocating each frame.

## What are some general C# performance tips?

1. **Measure first** with a profiler or `Stopwatch`; optimise only what shows up.
2. **Use the right collection**: `HashSet`/`Dictionary` for lookups instead of searching a `List`.
3. **Avoid allocations in hot loops**: reuse lists and buffers, avoid LINQ, boxing and string building there.
4. **Pre-size collections** when you know the size: `new List<T>(n)`, `new StringBuilder(capacity)`.
5. **Avoid exceptions for control flow**: `TryParse`, `TryGetValue`.
6. **Watch big structs**: pass them with `in`, or use classes; copying large structs is slow.
7. **Cache repeated work**: lookups, reflection, regexes (`RegexOptions.Compiled` or source-generated regexes).
8. **Parallelise CPU-bound work** with `Parallel.For` or tasks, and keep shared state minimal.
9. **Use `Span<T>`, `ArrayPool<T>` and `stackalloc`** in low-level parsing code.
10. **Build in Release**: Debug builds are much slower, and benchmarks belong in BenchmarkDotNet, not ad hoc timing.

## How do I write an object pool in C#?

A pool reuses objects instead of allocating new ones, which cuts garbage-collection work for objects that are created and thrown away often:

```csharp
public sealed class Pool<T> where T : class, new()
{
    private readonly Stack<T> free = new();
    private readonly Action<T> reset;

    public Pool(Action<T> reset = null) => this.reset = reset;

    public T Rent() => free.Count > 0 ? free.Pop() : new T();

    public void Return(T item)
    {
        reset?.Invoke(item);   // clear its state before reuse
        free.Push(item);
    }
}

var listPool = new Pool<List<int>>(l => l.Clear());
var tmp = listPool.Rent();
// ... use tmp ...
listPool.Return(tmp);
```

For thread safety use a `ConcurrentBag<T>` or lock around the stack. .NET also has `ArrayPool<T>.Shared` for arrays, and `Microsoft.Extensions.ObjectPool`.

## What causes a NullReferenceException in C# and how do I fix it?

It's thrown when you use a member of a reference that is `null`: calling a method, reading a property, indexing an array or list.

```csharp
Card card = FindCard("Mew");   // returned null
Console.WriteLine(card.Name);  // NullReferenceException
```

To fix it, find which reference is null (the debugger or the stack trace's line number), then work out why: a field never assigned, a lookup that found nothing, an object not initialised yet (in Unity: an Inspector reference not set, or using something in `Awake` before another object set it up). Then either make sure it's assigned, or handle the null case:

```csharp
if (card == null) return;
var name = card?.Name ?? "unknown";
if (!dict.TryGetValue(key, out var value)) { /* handle missing */ }
```

Enabling nullable reference types makes the compiler warn about most of these ahead of time.

## What does C# compiler error CS0103 'The name does not exist in the current context' mean?

You used a name the compiler can't see from that spot. Common causes:

- A typo or wrong capitalisation (C# is case-sensitive: `damage` and `Damage` are different).
- The variable was declared inside a block (`if`, `for`, `try`) and you're using it outside:

```csharp
if (ok) { int hp = 60; }
Console.WriteLine(hp);   // CS0103: hp only exists inside the braces
```

- A missing `using` for a static helper, or calling an instance member from a `static` method.
- The member is in another class: qualify it (`CardsDS.PowerReady(ctx)`).

Declare the variable in the scope where you need it, or fix the name.

## What do C# errors CS0246 and CS1061 mean?

- **CS0246: "The type or namespace name 'X' could not be found"**: the compiler doesn't know the type. Usually a missing `using` (`using System.Linq;`, `using System.Collections.Generic;`), a typo, or a missing reference to the assembly or package that contains it (in Unity: a missing package or assembly definition reference).
- **CS1061: "'Type' does not contain a definition for 'Member'"**: the type exists but has no such member. Usually a typo, the wrong type (you have the base type and need a cast), a missing `using` for an extension method (e.g. `.Where` without `using System.Linq;`), or a member that's private or doesn't exist in that version of the library.

## What do C# errors CS0029, CS0266 and CS0165 mean?

- **CS0029: "Cannot implicitly convert type 'A' to 'B'"**: the types don't match and there's no automatic conversion. Convert explicitly (`int.Parse(text)`, `.ToString()`, `.ToList()`) or fix the variable's type.
- **CS0266: "Cannot implicitly convert ... An explicit conversion exists (are you missing a cast?)"**: the conversion might lose data or fail, so you must ask for it: `int n = (int)someDouble;`, `var mon = (Mon)script;` (or use `as`/`is` to check safely).
- **CS0165: "Use of unassigned local variable"**: a local variable might be read before it's given a value, for example when it's only assigned inside an `if`. Initialise it when you declare it (`int best = 0;`) or make sure every path assigns it.

## What does error CS0120 'An object reference is required for the non-static field, method, or property' mean?

You're using an instance member without an instance, usually from a `static` method or through the class name:

```csharp
public class Engine
{
    public int Turn;
    public static void Log() => Console.WriteLine(Turn);   // CS0120: whose Turn?
}

Engine.Turn = 3;   // CS0120 as well
```

Fixes: use an instance (`var engine = new Engine(); engine.Turn = 3;`), pass the instance into the static method as a parameter, or, if the value really should be shared by all instances, make the member `static` too.

## What causes a StackOverflowException in C#?

Almost always unbounded recursion: a method that keeps calling itself (directly or through other methods) without reaching a stopping point, until the thread's stack runs out. A stack overflow can't be caught in .NET; the process simply dies.

Common causes:
- A property that returns itself: `public int Hp { get => Hp; }` (meant to return a backing field `hp`).
- Two objects calling each other back and forth, such as two "copy the other's attack" effects copying each other forever.
- Recursion over deep or cyclic data (a graph with a cycle and no "visited" set).

Fix the base case, add a depth limit or a visited set, or rewrite deep recursion as a loop with an explicit `Stack<T>`.

## What does IndexOutOfRangeException or ArgumentOutOfRangeException mean?

You used an index outside the valid range: below 0, or at or beyond the length (arrays throw `IndexOutOfRangeException`, `List<T>` throws `ArgumentOutOfRangeException`).

```csharp
var list = new List<int> { 1, 2, 3 };
var x = list[3];                       // valid indexes are 0..2
for (int i = 0; i <= list.Count; i++)  // off-by-one: should be i < list.Count
```

Other common causes: using an index from one list on another, a list that changed size after you computed the index, and `Random.Next(0, count)` with `count` 0. Check `Count` before indexing, use `foreach` when you don't need the index, and use `^1` (C# 8 index from end) for the last element: `list[^1]`.

## What are some newer C# features (C# 10 to 12) worth knowing?

- **File-scoped namespaces** (C# 10): `namespace PokemonTCG.Core;` at the top instead of wrapping the whole file in braces.
- **Global usings** (C# 10): `global using System.Linq;` once for the whole project.
- **Record structs** (C# 10): `public readonly record struct Point(int X, int Y);`.
- **Raw string literals** (C# 11): text in three or more double quotes, with no escaping needed.
- **`required` members** (C# 11): callers must set them in the initializer.
- **List patterns** (C# 11): `if (args is [var first, .., var last])`.
- **Primary constructors for classes** (C# 12): `public class Engine(ILogger log) { void Run() => log.Info("go"); }`.
- **Collection expressions** (C# 12): `int[] a = [1, 2, 3]; List<string> names = [..others, "Mew"];`.

Which features you can use depends on the language version your project targets; Unity supports up to C# 9.

## What are the common C# naming conventions?

- `PascalCase` for types, methods, properties, events, constants and public fields: `GameEngine`, `PlayTurn()`, `MaxBench`.
- `camelCase` for local variables and parameters: `damage`, `targetPokemon`.
- Private fields: `camelCase`, often with an underscore prefix (`_turnNumber`); either style is fine if it's consistent.
- Interfaces start with `I`: `IPlayerAgent`. Type parameters start with `T`: `TKey`, `TResult`.
- Async methods end in `Async`: `LoadDeckAsync`.
- Booleans read as yes/no questions: `IsActive`, `HasEnergy`, `CanRetreat`.

Prefer clear full words over abbreviations, and one public type per file, with the file named after it.

## What's the relationship between C# versions and .NET versions?

The C# language version and the .NET runtime version are related but separate:

- .NET Framework 4.x (Windows-only, legacy): C# 7.3 by default.
- .NET Core 3.x: C# 8. .NET 5: C# 9. .NET 6: C# 10. .NET 7: C# 11. .NET 8: C# 12. .NET 9: C# 13.
- Unity uses its own runtime (Mono or IL2CPP) with .NET Standard 2.1 APIs. Unity 2021.2 and later support C# 9, minus a few features that need runtime support (for example, `init` setters need a small `IsExternalInit` workaround, and covariant return types aren't supported).

Some language features need runtime support (default interface methods, `Span` performance), but many are compiler-only and can work on older targets. You can set `<LangVersion>` in a project, but using features newer than the runtime supports can fail.

## Write a C# method that shuffles a list fairly.

Use the Fisher–Yates shuffle: walk from the end, swapping each item with a random one at or before it. Every order is equally likely, and it runs in linear time:

```csharp
public static void Shuffle<T>(IList<T> list, Random rng)
{
    for (int i = list.Count - 1; i > 0; i--)
    {
        int j = rng.Next(i + 1);            // 0..i inclusive
        (list[i], list[j]) = (list[j], list[i]);
    }
}

var rng = new Random(seed);   // a fixed seed gives a reproducible shuffle
Shuffle(deck, rng);
```

Common mistakes: `rng.Next(list.Count)` for every position (biased), and `list.OrderBy(_ => rng.Next())` (works, but slower and allocates).

## Write a binary search in C#.

Binary search finds an item in a **sorted** list by halving the search range each step (logarithmic time):

```csharp
public static int BinarySearch(int[] sorted, int target)
{
    int lo = 0, hi = sorted.Length - 1;
    while (lo <= hi)
    {
        int mid = lo + (hi - lo) / 2;      // avoids overflow of (lo + hi)
        if (sorted[mid] == target) return mid;
        if (sorted[mid] < target) lo = mid + 1;
        else hi = mid - 1;
    }
    return -1;   // not found
}
```

In real code use the built-ins: `Array.BinarySearch(array, value)` and `list.BinarySearch(value)`. They return the index, or a negative number whose bitwise complement (`~result`) is where the item would be inserted.

## Write a simple state machine in C#.

An enum for the states plus a `switch` that handles each one is often enough:

```csharp
public enum TurnPhase { Draw, Main, Attack, Checkup, End }

public class Turn
{
    public TurnPhase Phase { get; private set; } = TurnPhase.Draw;

    public void Advance()
    {
        Phase = Phase switch
        {
            TurnPhase.Draw => TurnPhase.Main,
            TurnPhase.Main => TurnPhase.Attack,
            TurnPhase.Attack => TurnPhase.Checkup,
            TurnPhase.Checkup => TurnPhase.End,
            _ => TurnPhase.Draw,
        };
        Console.WriteLine($"Now in {Phase}");
    }
}
```

When each state has a lot of behaviour, use the State pattern instead: an `IState` interface with `Enter`, `Update` and `Exit` methods, one class per state, and a machine that holds the current state and switches between them.

## Write a C# method that counts how often each word appears in a text.

```csharp
public static Dictionary<string, int> WordCounts(string text)
{
    var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    var words = text.Split(new[] { ' ', '\n', '\r', '\t', '.', ',', '!', '?', ';', ':' },
                           StringSplitOptions.RemoveEmptyEntries);
    foreach (var word in words)
        counts[word] = counts.TryGetValue(word, out int n) ? n + 1 : 1;
    return counts;
}

foreach (var (word, count) in WordCounts(text).OrderByDescending(kv => kv.Value).Take(10))
    Console.WriteLine($"{word}: {count}");   // KeyValuePair deconstructs (.NET Core 2.0+)
```

The `StringComparer.OrdinalIgnoreCase` comparer makes "Pikachu" and "pikachu" count as the same word. A LINQ version is shorter but allocates more: `words.GroupBy(w => w.ToLowerInvariant()).ToDictionary(g => g.Key, g => g.Count())`.

## How do I read a CSV file into objects in C#?

For simple CSV (no commas or quotes inside fields), split each line:

```csharp
public record CardRow(string Name, int Hp, string Type);

var rows = File.ReadLines("cards.csv")
    .Skip(1)                                  // header line
    .Where(line => !string.IsNullOrWhiteSpace(line))
    .Select(line => line.Split(','))
    .Select(f => new CardRow(f[0].Trim(), int.Parse(f[1], CultureInfo.InvariantCulture), f[2].Trim()))
    .ToList();
```

Real-world CSV often has quoted fields containing commas and line breaks, which simple `Split` gets wrong. For that, use a library such as CsvHelper (`csv.GetRecords<CardRow>()`) or `Microsoft.VisualBasic.FileIO.TextFieldParser`. Parse numbers with `CultureInfo.InvariantCulture`, so it works on PCs with other regional settings.
