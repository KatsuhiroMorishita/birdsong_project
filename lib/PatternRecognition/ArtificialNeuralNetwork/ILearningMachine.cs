using System;
namespace PatternRecognition.ArtificialNeuralNetwork
{
    /// <summary>
    /// 将来的にSVMの搭載も考えてインターフェイスを考察するために抽出してみたpublicたち
    /// 現時点ではほぼ意味がない。特に引数のあたり。
    /// </summary>
    interface ILearningMachine
    {
        Parameter GetParameter();
        PatternRecognition.Vector Learn(PatternRecognition.Feature featureVector, PatternRecognition.Vector NeuralNetTeacher);
        double Learn(PatternRecognition.Feature featureVector, double NeuralNetTeacher);
        double Learn(double feature, double NeuralNetTeacher);
        double Learn(double[] featureVector, double NeuralNetTeacher);
        double[] Learn(double[] featureVector, double[] NeuralNetTeacher);
        PatternRecognition.Vector Recognize(PatternRecognition.Feature featureVector);
        double Recognize(double feature);
        double[] Recognize(double[] featureVector);
        void Save(string fname = "NN.ini", bool append = true);
        void Setup(string fname = "NN.ini");
        string ToString();
        double TotalOutputError { get; }
        double VariationOfError { get; }
    }
}
