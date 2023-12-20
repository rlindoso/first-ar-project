public class UserExperiment
{
    // Propriedades da entidade
    public int experimentNo { get; set; }
    public string experimentName { get; set; }
    public string userId { get; set; }

    // Construtor
    public UserExperiment(int experimentoNo, string experimentName, string userId)
    {
        this.experimentNo = experimentoNo;
        this.experimentName = experimentName;
        this.userId = userId;
    }
}
