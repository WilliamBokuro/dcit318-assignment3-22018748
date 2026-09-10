using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystem
{
    // a. Generic Repository for entity management
    public class Repository<T>
    {
        private List<T> items = new List<T>();

        public void Add(T item)
        {
            items.Add(item);
        }

        public List<T> GetAll()
        {
            return items;
        }

        public T? GetById(Func<T, bool> predicate)
        {
            return items.FirstOrDefault(predicate);
        }

        public bool Remove(Func<T, bool> predicate)
        {
            T? item = GetById(predicate);
            if (item != null)
            {
                return items.Remove(item);
            }
            return false;
        }
    }

    // b. Patient Class
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }
    }

    // c. Prescription Class
    public class Prescription
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string MedicationName { get; set; }
        public DateTime DateIssued { get; set; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }
    }

    // g. HealthSystemApp Class
    public class HealthSystemApp
    {
        private Repository<Patient> _patientRepo = new Repository<Patient>();
        private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

        public void SeedData()
        {
            // Seed Patients
            _patientRepo.Add(new Patient(101, "Kwame Mensah", 34, "Male"));
            _patientRepo.Add(new Patient(102, "Ama Serwaa", 28, "Female"));
            _patientRepo.Add(new Patient(103, "Kofi Osei", 45, "Male"));

            // Seed Prescriptions
            _prescriptionRepo.Add(new Prescription(1, 101, "Amoxicillin", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(2, 101, "Ibuprofen", DateTime.Now.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(3, 102, "Paracetamol", DateTime.Now.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(4, 102, "Metformin", DateTime.Now.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(5, 103, "Lisinopril", DateTime.Now));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();
            List<Prescription> allPrescriptions = _prescriptionRepo.GetAll();

            foreach (var prescription in allPrescriptions)
            {
                if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                {
                    _prescriptionMap[prescription.PatientId] = new List<Prescription>();
                }
                _prescriptionMap[prescription.PatientId].Add(prescription);
            }
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            if (_prescriptionMap.TryGetValue(patientId, out var prescriptions))
            {
                return prescriptions;
            }
            return new List<Prescription>();
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("=== Patient List ===");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine($"ID: {patient.Id} | Name: {patient.Name} | Age: {patient.Age} | Gender: {patient.Gender}");
            }
            Console.WriteLine();
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            Patient? patient = _patientRepo.GetById(p => p.Id == id);
            string patientName = patient != null ? patient.Name : "Unknown";

            Console.WriteLine($"=== Prescriptions for Patient ID: {id} ({patientName}) ===");
            List<Prescription> list = GetPrescriptionsByPatientId(id);

            if (list.Count == 0)
            {
                Console.WriteLine("No prescriptions found.");
            }
            else
            {
                foreach (var p in list)
                {
                    Console.WriteLine($"Rx ID: {p.Id} | Medication: {p.MedicationName} | Date: {p.DateIssued:yyyy-MM-dd}");
                }
            }
            Console.WriteLine();
        }

        public static void Main(string[] args)
        {
            HealthSystemApp app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();

            app.PrintAllPatients();
            app.PrintPrescriptionsForPatient(101);
        }
    }
}