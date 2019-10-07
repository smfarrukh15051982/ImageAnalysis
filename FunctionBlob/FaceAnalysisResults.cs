using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionBlob
{
    public class FaceAnalysisResults
    {
        public string ImageId { get; set; }
        public Face[] Faces { get; set; }
    }
}
