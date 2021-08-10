using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration; 

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Azure.Storage;

namespace patientrecords.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientRecordsController : ControllerBase
    {
        private readonly ILogger<PatientRecordsController> _logger;
        private IConfiguration _iconfiguration;

        private BlobContainerClient _container;
 

        public PatientRecordsController(ILogger<PatientRecordsController> logger, IConfiguration iconfiguration)
        {
            _logger = logger;
            _iconfiguration = iconfiguration;

            // Azure Blob storage client library v12
            _container = new BlobContainerClient(
                _iconfiguration.GetValue<string>("StorageAccount:ConnectionString"),
                _iconfiguration.GetValue<string>("StorageAccount:Container")
            );
        }

        // GET PatientRecord
        [HttpGet]
        public IEnumerable<PatientRecord> Get()
        {
            List<PatientRecord> records = new List<PatientRecord>();

            foreach (BlobItem blobItem in _container.GetBlobs())
            {
                BlobClient blob = _container.GetBlobClient(blobItem.Name);
                var patient = new PatientRecord { name=blob.Name, imageURI=blob.Uri.ToString() };
                records.Add(patient);
            }

            return records;
        }

        // GET PatientRecord/patient-nnnnnn
        [HttpGet("{Name}")]
        public PatientRecord Get(string name)
        {
            BlobClient blob = _container.GetBlobClient(name);
            return new PatientRecord { name=blob.Name, imageURI=blob.Uri.AbsoluteUri };
        }
    }
}
