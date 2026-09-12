using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystem
{
    // Generic repo that works for any entity type
    public class Repository<T>
    {
        private readonly List<T> _items = new();

        public void Add(T item)
        {
            _items.Add(item);
        }

        public List<T> GetAll()
        {
            return _items;
        }

        // Returns the first item matching the condition, or null
        public T? GetById(Func<T, bool> predicate)
        {
            return _items.FirstOrDefault(predicate);
        }

        public bool Remove(Func<T, bool> predicate)
        {
            var item = _items.FirstOrDefault(predicate);
            if (item is null) return false;
            _items.Remove(item);
            return true;
        }
    }

    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }
    }

    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }
    }

    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Ama Mensah", 28, "Female"));
            _patientRepo.Add(new Patient(2, "Kwame Boateng", 45, "Male"));
            _patientRepo.Add(new Patient(3, "Efua Asante", 33, "Female"));

            _prescriptionRepo.Add(new Prescription(101, 1, "Paracetamol", DateTime.Now.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(102, 1, "Amoxicillin", DateTime.Now.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(103, 2, "Ibuprofen", DateTime.Now.AddDays(-3)));
            _prescriptionRepo.Add(new Prescription(104, 3, "Vitamin C", DateTime.Now.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(105, 3, "Cetirizine", DateTime.Now));
        }

        // Group all prescriptions by patient id for quick lookup
        public void BuildPrescriptionMap()
        {
            foreach (var prescription in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                {
                    _prescriptionMap[prescription.PatientId] = new List<Prescription>();
                }
                _prescriptionMap[prescription.PatientId].Add(prescription);
            }
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("=== ALL PATIENTS ===");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine($"ID: {patient.Id} | Name: {patient.Name} | Age: {patient.Age} | Gender: {patient.Gender}");
            }
            Console.WriteLine();
        }

        public void PrintPrescriptionsForPatient(int patientId)
        {
            var patient = _patientRepo.GetById(p => p.Id == patientId);

            if (patient is null)
            {
                Console.WriteLine($"No patient found with ID {patientId}.");
                return;
            }

            Console.WriteLine($"=== PRESCRIPTIONS FOR {patient.Name} (ID: {patient.Id}) ===");

            if (!_prescriptionMap.TryGetValue(patientId, out var prescriptions))
            {
                Console.WriteLine("No prescriptions on record.");
                return;
            }

            foreach (var prescription in prescriptions)
            {
                Console.WriteLine($"Rx ID: {prescription.Id} | Medication: {prescription.MedicationName} | Issued: {prescription.DateIssued:d}");
            }
        }
    }

    public class Program
    {
        public static void Main()
        {
            var app = new HealthSystemApp();

            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();
            app.PrintPrescriptionsForPatient(1);
        }
    }
}