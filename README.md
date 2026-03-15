For Contibutors
============

### Resources
1. Add resources to the project directory. Do not forget to choose 'EmbeddedRecource' in 'Build Action' and 'Copy always' in 'Copy to output directory' in File Properties dialog. 
   
### C# files
1. Do not forget to comment each function.
```
	/**
	* <summary>Returns the first literal's name.</summary>
	*
	* <returns>the first literal's name.</returns>
	*/
	public string Representative()
	{
		return GetSynonym().GetLiteral(0).GetName();
	}
```
2. Function names should follow pascal caml case.
```
	public string GetLongDefinition()
```
3. Write ToString methods, if necessary.
4. Use var type as a standard type.
```
	public override bool Equals(object second)
	{
		var relation = (Relation) second;
```
5. Use standard naming for private and protected class variables. Use _ for private and capital for protected class members.
```
    public class SynSet
    {
        private string _id;
		protected string Name;
```
6. Use NUnit for writing test classes. Use test setup if necessary.
```
   public class WordNetTest
    {
        WordNet.WordNet turkish;

        [SetUp]
        public void Setup()
        {
            turkish = new WordNet.WordNet();
        }

        [Test]
        public void TestSynSetList()
        {
            var literalCount = 0;
            foreach (var synSet in turkish.SynSetList()){
                literalCount += synSet.GetSynonym().LiteralSize();
            }
            Assert.AreEqual(110259, literalCount);
        }
```
