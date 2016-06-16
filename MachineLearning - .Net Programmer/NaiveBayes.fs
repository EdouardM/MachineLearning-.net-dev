namespace MachineLearning.NaiveBayes

module Classifier =

    type Token = string
    type Tokenizer = string -> Token Set
    type TokenizedDoc = Token Set

    /// <summary>
    /// Contains a Proportion (how frequently the
    /// group (ex: Spam or Ham) is found in the overall data
    /// set), and TokenFrequencies, a Map that
    /// associates each token with its Laplace-corrected
    /// frequency in the dataset.
    /// </summary>
    type DocsGroup = {
        Proportion: float ;
        TokenFrequencies: Map<Token,float> }

    let tokenScore (group: DocsGroup) (token: Token) =
        if group.TokenFrequencies.ContainsKey(token) then
            log group.TokenFrequencies.[token]
        else 0.0

    let score (document: TokenizedDoc) (group: DocsGroup) =
        let scoreToken = tokenScore group
        log group.Proportion +
        (document |> Seq.sumBy scoreToken)

    /// <summary>
    /// Classify a document by:
    /// - Transforming into tokens using tokenizer
    /// - Identify group with highest score in list
    /// of DocsGroup
    /// - Return Group label
    /// </summary>
    let classify    (groups: ('a * DocsGroup) [])
                    (tokenizer: Tokenizer)
                    (txt: string) =
        let tokenized = tokenizer txt
        groups
        |> Array.maxBy(fun (label, grp) -> score tokenized grp)
        |> fst

    ///Learn from corpus of documents

    let proportion count total = float count / float total
    let laplace count total = float (count + 1)/ float (total + 1)

    let countIn (group: TokenizedDoc seq) (token: Token) =
        group
        |> Seq.filter(fun doc -> doc.Contains token)
        |> Seq.length

    /// <summary>
    /// Analyse a group of documents that have
    /// the same label and summarize in DocsGroup:
    /// - compute the relative size of the group, compared to total number of documents
    /// - find for every token their laplace proportion in group
    /// </summary>
    let analyze (group: TokenizedDoc seq)
                (totalDocs: int)
                (classificationTokens: Token Set) =
        let groupSize = group |> Seq.length

        ///Relative size of group vs. total docs
        let groupProportion = proportion groupSize totalDocs

        ///function to compute Laplace proportion in group
        let score token =
            let count = countIn group token
            laplace count groupSize

        ///Score every token
        let scoredTokens =
            classificationTokens
            |> Seq.map (fun token -> token, score token)
            |> Map.ofSeq

        //Return DocsGroup
        {
            Proportion       = groupProportion ;
            TokenFrequencies = scoredTokens
        }


    /// <summary>
    /// Given a collection of documents and their label we
    /// need to:
    /// - break each docs into tokens
    /// - separate docs in groups by label
    /// - analyse every group
    /// </summary>
    let learn   ( docs: ( 'a * string) [] )
                ( tokenizer: Tokenizer)
                ( classificationTokens: Token Set) =
        let total = docs.Length
        docs
        |> Array.map(fun (label, txt) -> label, tokenizer txt)
        |> Seq.groupBy fst
        |> Seq.map(fun (label, group) -> label, group |> Seq.map snd)
        |> Seq.map(fun (label, group) -> label, analyze group total classificationTokens)
        |> Array.ofSeq

    /// <summary>
    /// Returns a classifier for new docs
    /// based on what we learned from documents in training Set
    /// </summary>
    let train   ( docs: ( 'a * string) [] )
                ( tokenizer: Tokenizer)
                ( classificationTokens: Token Set) =
        let groups = learn docs tokenizer classificationTokens
        let classifier = classify groups tokenizer
        classifier
