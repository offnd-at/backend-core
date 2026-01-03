using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Extensions;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.Models;

namespace OffndAt.Domain.ValueObjects;

/// <summary>
///     Represents the vocabulary value object.
/// </summary>
public sealed class Vocabulary : ValueObject
{
    private readonly Random _random = new(Guid.NewGuid().GetHashCode());
    private readonly List<Word> _words = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="Vocabulary" /> class.
    /// </summary>
    /// <param name="descriptor">The vocabulary descriptor.</param>
    /// <param name="words">The collection of words.</param>
    private Vocabulary(VocabularyDescriptor descriptor, IEnumerable<Word> words)
    {
        Language = descriptor.Language;
        Theme = descriptor.Theme;
        Offensiveness = descriptor.Offensiveness;
        GrammaticalNumber = descriptor.GrammaticalNumber;
        GrammaticalGender = descriptor.GrammaticalGender;
        PartOfSpeech = descriptor.PartOfSpeech;
        _words.AddRange(words);
    }

    /// <summary>
    ///     Gets the language.
    /// </summary>
    public Language Language { get; }

    /// <summary>
    ///     Gets the theme.
    /// </summary>
    public Theme Theme { get; }

    /// <summary>
    ///     Gets the offensiveness.
    /// </summary>
    public Offensiveness Offensiveness { get; }

    /// <summary>
    ///     Gets the grammatical number.
    /// </summary>
    public GrammaticalNumber GrammaticalNumber { get; }

    /// <summary>
    ///     Gets the grammatical gender.
    /// </summary>
    public GrammaticalGender GrammaticalGender { get; }

    /// <summary>
    ///     Gets the part of speech.
    /// </summary>
    public PartOfSpeech PartOfSpeech { get; }

    /// <summary>
    ///     Gets the list of words.
    /// </summary>
    public IReadOnlyList<Word> Words =>
        _words.AsReadOnly();

    /// <summary>
    ///     Gets the random word from the list of words.
    /// </summary>
    public Word RandomWord =>
        _words[_random.Next(_words.Count)];

    /// <summary>
    ///     Creates a new <see cref="Vocabulary" /> instance based on the specified arguments.
    /// </summary>
    /// <param name="descriptor">The vocabulary descriptor.</param>
    /// <param name="words">The collection of words.</param>
    /// <returns>The newly created <see cref="Vocabulary" /> instance.</returns>
    /// <returns>The result of the value object creation process containing the <see cref="Vocabulary" /> or an <see cref="Error" />.</returns>
    public static Result<Vocabulary> Create(VocabularyDescriptor descriptor, IReadOnlyList<Word> words) =>
        Result.Create(words, DomainErrors.Vocabulary.EmptyWordsList)
            .Ensure(value => value.Any(), DomainErrors.Vocabulary.EmptyWordsList)
            .Map(_ => new Vocabulary(descriptor, words));

    /// <inheritdoc />
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Language;
        yield return Theme;
        yield return Offensiveness;
        yield return GrammaticalNumber;
        yield return GrammaticalGender;
        yield return PartOfSpeech;
        yield return Words;
    }
}
