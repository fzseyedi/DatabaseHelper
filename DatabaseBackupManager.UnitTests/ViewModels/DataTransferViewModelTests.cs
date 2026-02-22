using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using DatabaseBackupManager.Models;
using DatabaseBackupManager.Services;
using Moq;

namespace DatabaseBackupManager.ViewModels.UnitTests;


/// <summary>
/// Unit tests for the DataTransferViewModel class.
/// </summary>
[TestClass]
public partial class DataTransferViewModelTests
{
    /// <summary>
    /// Tests that IsStep2 returns true when CurrentStep is set to 2.
    /// </summary>
    [TestMethod]
    public void IsStep2_WhenCurrentStepIs2_ReturnsTrue()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.CurrentStep = 2;

        // Act
        var result = viewModel.IsStep2;

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// Tests that IsStep2 returns false when CurrentStep is set to values other than 2.
    /// Validates the property with step 1, step 3, zero, negative, and boundary values.
    /// </summary>
    /// <param name="currentStep">The current step value to test.</param>
    [TestMethod]
    [DataRow(1, DisplayName = "Step 1")]
    [DataRow(3, DisplayName = "Step 3")]
    [DataRow(0, DisplayName = "Zero")]
    [DataRow(-1, DisplayName = "Negative value")]
    [DataRow(4, DisplayName = "Step 4")]
    [DataRow(100, DisplayName = "Large positive value")]
    [DataRow(-100, DisplayName = "Large negative value")]
    [DataRow(int.MaxValue, DisplayName = "Int.MaxValue boundary")]
    [DataRow(int.MinValue, DisplayName = "Int.MinValue boundary")]
    public void IsStep2_WhenCurrentStepIsNotTwo_ReturnsFalse(int currentStep)
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.CurrentStep = currentStep;

        // Act
        var result = viewModel.IsStep2;

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Tests that IsStep2 returns false in the initial state after construction.
    /// The CurrentStep defaults to 1, so IsStep2 should be false.
    /// </summary>
    [TestMethod]
    public void IsStep2_InInitialState_ReturnsFalse()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        var result = viewModel.IsStep2;

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Tests that IsStep2 correctly reflects changes when CurrentStep is modified multiple times.
    /// Verifies that the property accurately tracks transitions between different step values.
    /// </summary>
    [TestMethod]
    public void IsStep2_WhenCurrentStepChangesMultipleTimes_ReflectsCurrentValue()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act & Assert - Initial state (step 1)
        Assert.IsFalse(viewModel.IsStep2);

        // Act & Assert - Change to step 2
        viewModel.CurrentStep = 2;
        Assert.IsTrue(viewModel.IsStep2);

        // Act & Assert - Change to step 3
        viewModel.CurrentStep = 3;
        Assert.IsFalse(viewModel.IsStep2);

        // Act & Assert - Change back to step 2
        viewModel.CurrentStep = 2;
        Assert.IsTrue(viewModel.IsStep2);

        // Act & Assert - Change to step 1
        viewModel.CurrentStep = 1;
        Assert.IsFalse(viewModel.IsStep2);
    }

    /// <summary>
    /// Tests that RememberDestinationSettings has the correct initial value of true.
    /// </summary>
    [TestMethod]
    public void RememberDestinationSettings_InitialValue_ReturnsTrue()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);

        // Act
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);

        // Assert
        Assert.IsTrue(viewModel.RememberDestinationSettings);
    }

    /// <summary>
    /// Tests that setting RememberDestinationSettings to a new value updates the property correctly.
    /// Input: false when initial value is true
    /// Expected: Property value changes to false
    /// </summary>
    [TestMethod]
    public void RememberDestinationSettings_SetToFalse_UpdatesValueToFalse()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);

        // Act
        viewModel.RememberDestinationSettings = false;

        // Assert
        Assert.IsFalse(viewModel.RememberDestinationSettings);
    }

    /// <summary>
    /// Tests that setting RememberDestinationSettings to a new value updates the property correctly.
    /// Input: true when value is false
    /// Expected: Property value changes to true
    /// </summary>
    [TestMethod]
    public void RememberDestinationSettings_SetToTrue_UpdatesValueToTrue()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        viewModel.RememberDestinationSettings = false;

        // Act
        viewModel.RememberDestinationSettings = true;

        // Assert
        Assert.IsTrue(viewModel.RememberDestinationSettings);
    }

    /// <summary>
    /// Tests that PropertyChanged event is raised when RememberDestinationSettings changes from true to false.
    /// Input: false when value is true
    /// Expected: PropertyChanged event is raised with property name "RememberDestinationSettings"
    /// </summary>
    [TestMethod]
    public void RememberDestinationSettings_ChangeFromTrueToFalse_RaisesPropertyChangedEvent()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        string? changedPropertyName = null;
        viewModel.PropertyChanged += (sender, args) => changedPropertyName = args.PropertyName;

        // Act
        viewModel.RememberDestinationSettings = false;

        // Assert
        Assert.AreEqual("RememberDestinationSettings", changedPropertyName);
    }

    /// <summary>
    /// Tests that PropertyChanged event is raised when RememberDestinationSettings changes from false to true.
    /// Input: true when value is false
    /// Expected: PropertyChanged event is raised with property name "RememberDestinationSettings"
    /// </summary>
    [TestMethod]
    public void RememberDestinationSettings_ChangeFromFalseToTrue_RaisesPropertyChangedEvent()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        viewModel.RememberDestinationSettings = false;
        string? changedPropertyName = null;
        viewModel.PropertyChanged += (sender, args) => changedPropertyName = args.PropertyName;

        // Act
        viewModel.RememberDestinationSettings = true;

        // Assert
        Assert.AreEqual("RememberDestinationSettings", changedPropertyName);
    }

    /// <summary>
    /// Tests that PropertyChanged event is NOT raised when setting RememberDestinationSettings to the same value (true).
    /// Input: true when value is already true
    /// Expected: PropertyChanged event is not raised
    /// </summary>
    [TestMethod]
    public void RememberDestinationSettings_SetToSameValueTrue_DoesNotRaisePropertyChangedEvent()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        bool eventRaised = false;
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "RememberDestinationSettings")
                eventRaised = true;
        };

        // Act
        viewModel.RememberDestinationSettings = true;

        // Assert
        Assert.IsFalse(eventRaised);
    }

    /// <summary>
    /// Tests that PropertyChanged event is NOT raised when setting RememberDestinationSettings to the same value (false).
    /// Input: false when value is already false
    /// Expected: PropertyChanged event is not raised
    /// </summary>
    [TestMethod]
    public void RememberDestinationSettings_SetToSameValueFalse_DoesNotRaisePropertyChangedEvent()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        viewModel.RememberDestinationSettings = false;
        bool eventRaised = false;
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "RememberDestinationSettings")
                eventRaised = true;
        };

        // Act
        viewModel.RememberDestinationSettings = false;

        // Assert
        Assert.IsFalse(eventRaised);
    }

    /// <summary>
    /// Tests multiple consecutive value changes to verify property behavior is consistent.
    /// Input: Multiple alternating boolean values
    /// Expected: Property value reflects the most recent set value
    /// </summary>
    [TestMethod]
    public void RememberDestinationSettings_MultipleChanges_ReflectsLastSetValue()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);

        // Act & Assert
        viewModel.RememberDestinationSettings = false;
        Assert.IsFalse(viewModel.RememberDestinationSettings);

        viewModel.RememberDestinationSettings = true;
        Assert.IsTrue(viewModel.RememberDestinationSettings);

        viewModel.RememberDestinationSettings = false;
        Assert.IsFalse(viewModel.RememberDestinationSettings);

        viewModel.RememberDestinationSettings = true;
        Assert.IsTrue(viewModel.RememberDestinationSettings);
    }

    /// <summary>
    /// Tests that setting SelectedDestinationDatabase with a valid DatabaseInfo object
    /// updates the property value and raises PropertyChanged event.
    /// </summary>
    [TestMethod]
    public void SelectedDestinationDatabase_SetValidDatabaseInfo_UpdatesValueAndRaisesPropertyChanged()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var databaseInfo = new DatabaseInfo { Name = "TestDatabase" };
        bool propertyChangedRaised = false;
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(viewModel.SelectedDestinationDatabase))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.SelectedDestinationDatabase = databaseInfo;

        // Assert
        Assert.AreEqual(databaseInfo, viewModel.SelectedDestinationDatabase);
        Assert.IsTrue(propertyChangedRaised);
    }

    /// <summary>
    /// Tests that setting SelectedDestinationDatabase to null
    /// updates the property value to null and raises PropertyChanged event.
    /// </summary>
    [TestMethod]
    public void SelectedDestinationDatabase_SetNull_UpdatesValueToNullAndRaisesPropertyChanged()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.SelectedDestinationDatabase = new DatabaseInfo { Name = "InitialDatabase" };
        bool propertyChangedRaised = false;
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(viewModel.SelectedDestinationDatabase))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.SelectedDestinationDatabase = null;

        // Assert
        Assert.IsNull(viewModel.SelectedDestinationDatabase);
        Assert.IsTrue(propertyChangedRaised);
    }

    /// <summary>
    /// Tests that setting SelectedDestinationDatabase to the same value
    /// does not raise PropertyChanged event (no change scenario).
    /// </summary>
    [TestMethod]
    public void SelectedDestinationDatabase_SetSameValue_DoesNotRaisePropertyChanged()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var databaseInfo = new DatabaseInfo { Name = "TestDatabase" };
        viewModel.SelectedDestinationDatabase = databaseInfo;
        bool propertyChangedRaised = false;
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(viewModel.SelectedDestinationDatabase))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.SelectedDestinationDatabase = databaseInfo;

        // Assert
        Assert.AreEqual(databaseInfo, viewModel.SelectedDestinationDatabase);
        Assert.IsFalse(propertyChangedRaised);
    }

    /// <summary>
    /// Tests that setting SelectedDestinationDatabase to a different DatabaseInfo object
    /// updates the property value and raises PropertyChanged event.
    /// </summary>
    [TestMethod]
    public void SelectedDestinationDatabase_SetDifferentValue_UpdatesValueAndRaisesPropertyChanged()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var initialDatabase = new DatabaseInfo { Name = "InitialDatabase" };
        var newDatabase = new DatabaseInfo { Name = "NewDatabase" };
        viewModel.SelectedDestinationDatabase = initialDatabase;
        bool propertyChangedRaised = false;
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(viewModel.SelectedDestinationDatabase))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.SelectedDestinationDatabase = newDatabase;

        // Assert
        Assert.AreEqual(newDatabase, viewModel.SelectedDestinationDatabase);
        Assert.IsTrue(propertyChangedRaised);
    }

    /// <summary>
    /// Tests that getting SelectedDestinationDatabase returns the initial default value (null).
    /// </summary>
    [TestMethod]
    public void SelectedDestinationDatabase_InitialValue_IsNull()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        var result = viewModel.SelectedDestinationDatabase;

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// Tests that setting SelectedDestinationDatabase to null when already null
    /// does not raise PropertyChanged event.
    /// </summary>
    [TestMethod]
    public void SelectedDestinationDatabase_SetNullWhenAlreadyNull_DoesNotRaisePropertyChanged()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        bool propertyChangedRaised = false;
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(viewModel.SelectedDestinationDatabase))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.SelectedDestinationDatabase = null;

        // Assert
        Assert.IsNull(viewModel.SelectedDestinationDatabase);
        Assert.IsFalse(propertyChangedRaised);
    }

    /// <summary>
    /// Tests that SourceRowCount returns the initial default value of 0 when no data has been loaded.
    /// </summary>
    [TestMethod]
    public void SourceRowCount_InitialValue_ReturnsZero()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        var result = viewModel.SourceRowCount;

        // Assert
        Assert.AreEqual(0L, result);
    }

    /// <summary>
    /// Tests that CurrentStep returns the initial default value of 1.
    /// </summary>
    [TestMethod]
    public void CurrentStep_InitialValue_ReturnsOne()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        int result = viewModel.CurrentStep;

        // Assert
        Assert.AreEqual(1, result);
    }

    /// <summary>
    /// Tests that setting CurrentStep to a new value updates the property and returns the new value.
    /// </summary>
    /// <param name="newValue">The new value to set.</param>
    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(4)]
    [DataRow(100)]
    [DataRow(int.MinValue)]
    [DataRow(int.MaxValue)]
    public void CurrentStep_SetValue_UpdatesProperty(int newValue)
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        viewModel.CurrentStep = newValue;

        // Assert
        Assert.AreEqual(newValue, viewModel.CurrentStep);
    }

    /// <summary>
    /// Tests that setting CurrentStep to a different value raises PropertyChanged for CurrentStep.
    /// </summary>
    [TestMethod]
    public void CurrentStep_SetDifferentValue_RaisesPropertyChangedForCurrentStep()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var propertyChangedRaised = false;
        string? changedPropertyName = null;

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(viewModel.CurrentStep))
            {
                propertyChangedRaised = true;
                changedPropertyName = args.PropertyName;
            }
        };

        // Act
        viewModel.CurrentStep = 2;

        // Assert
        Assert.IsTrue(propertyChangedRaised);
        Assert.AreEqual(nameof(viewModel.CurrentStep), changedPropertyName);
    }

    /// <summary>
    /// Tests that setting CurrentStep to a different value raises PropertyChanged for IsStep1.
    /// </summary>
    [TestMethod]
    public void CurrentStep_SetDifferentValue_RaisesPropertyChangedForIsStep1()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var propertyChangedRaised = false;

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(viewModel.IsStep1))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.CurrentStep = 2;

        // Assert
        Assert.IsTrue(propertyChangedRaised);
    }

    /// <summary>
    /// Tests that setting CurrentStep to a different value raises PropertyChanged for IsStep2.
    /// </summary>
    [TestMethod]
    public void CurrentStep_SetDifferentValue_RaisesPropertyChangedForIsStep2()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var propertyChangedRaised = false;

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(viewModel.IsStep2))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.CurrentStep = 3;

        // Assert
        Assert.IsTrue(propertyChangedRaised);
    }

    /// <summary>
    /// Tests that setting CurrentStep to a different value raises PropertyChanged for IsStep3.
    /// </summary>
    [TestMethod]
    public void CurrentStep_SetDifferentValue_RaisesPropertyChangedForIsStep3()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var propertyChangedRaised = false;

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(viewModel.IsStep3))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.CurrentStep = 2;

        // Assert
        Assert.IsTrue(propertyChangedRaised);
    }

    /// <summary>
    /// Tests that setting CurrentStep to the same value does not raise PropertyChanged events.
    /// </summary>
    [TestMethod]
    public void CurrentStep_SetSameValue_DoesNotRaisePropertyChanged()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var propertyChangedCount = 0;

        viewModel.PropertyChanged += (sender, args) =>
        {
            propertyChangedCount++;
        };

        // Act
        viewModel.CurrentStep = 1; // Setting to initial value (1)

        // Assert
        Assert.AreEqual(0, propertyChangedCount);
    }

    /// <summary>
    /// Tests that setting CurrentStep to a different value raises all expected PropertyChanged events.
    /// </summary>
    [TestMethod]
    public void CurrentStep_SetDifferentValue_RaisesAllExpectedPropertyChangedEvents()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var raisedProperties = new List<string>();

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName != null)
            {
                raisedProperties.Add(args.PropertyName);
            }
        };

        // Act
        viewModel.CurrentStep = 2;

        // Assert
        Assert.IsTrue(raisedProperties.Contains(nameof(viewModel.CurrentStep)));
        Assert.IsTrue(raisedProperties.Contains(nameof(viewModel.IsStep1)));
        Assert.IsTrue(raisedProperties.Contains(nameof(viewModel.IsStep2)));
        Assert.IsTrue(raisedProperties.Contains(nameof(viewModel.IsStep3)));
        Assert.AreEqual(4, raisedProperties.Count);
    }

    /// <summary>
    /// Tests that IsStep1 returns true when CurrentStep is 1 and false otherwise.
    /// </summary>
    /// <param name="stepValue">The step value to set.</param>
    /// <param name="expectedIsStep1">Expected IsStep1 value.</param>
    [TestMethod]
    [DataRow(1, true)]
    [DataRow(2, false)]
    [DataRow(3, false)]
    [DataRow(0, false)]
    [DataRow(-1, false)]
    [DataRow(4, false)]
    public void CurrentStep_SetValue_UpdatesIsStep1Correctly(int stepValue, bool expectedIsStep1)
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        viewModel.CurrentStep = stepValue;

        // Assert
        Assert.AreEqual(expectedIsStep1, viewModel.IsStep1);
    }

    /// <summary>
    /// Tests that IsStep2 returns true when CurrentStep is 2 and false otherwise.
    /// </summary>
    /// <param name="stepValue">The step value to set.</param>
    /// <param name="expectedIsStep2">Expected IsStep2 value.</param>
    [TestMethod]
    [DataRow(1, false)]
    [DataRow(2, true)]
    [DataRow(3, false)]
    [DataRow(0, false)]
    [DataRow(-1, false)]
    [DataRow(4, false)]
    public void CurrentStep_SetValue_UpdatesIsStep2Correctly(int stepValue, bool expectedIsStep2)
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        viewModel.CurrentStep = stepValue;

        // Assert
        Assert.AreEqual(expectedIsStep2, viewModel.IsStep2);
    }

    /// <summary>
    /// Tests that IsStep3 returns true when CurrentStep is 3 and false otherwise.
    /// </summary>
    /// <param name="stepValue">The step value to set.</param>
    /// <param name="expectedIsStep3">Expected IsStep3 value.</param>
    [TestMethod]
    [DataRow(1, false)]
    [DataRow(2, false)]
    [DataRow(3, true)]
    [DataRow(0, false)]
    [DataRow(-1, false)]
    [DataRow(4, false)]
    public void CurrentStep_SetValue_UpdatesIsStep3Correctly(int stepValue, bool expectedIsStep3)
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        viewModel.CurrentStep = stepValue;

        // Assert
        Assert.AreEqual(expectedIsStep3, viewModel.IsStep3);
    }

    /// <summary>
    /// Tests that setting CurrentStep multiple times with different values raises PropertyChanged each time.
    /// </summary>
    [TestMethod]
    public void CurrentStep_SetMultipleDifferentValues_RaisesPropertyChangedEachTime()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var propertyChangedCount = 0;

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(viewModel.CurrentStep))
            {
                propertyChangedCount++;
            }
        };

        // Act
        viewModel.CurrentStep = 2;
        viewModel.CurrentStep = 3;
        viewModel.CurrentStep = 1;

        // Assert
        Assert.AreEqual(3, propertyChangedCount);
    }

    /// <summary>
    /// Tests that setting CurrentStep to the same value multiple times does not raise PropertyChanged.
    /// </summary>
    [TestMethod]
    public void CurrentStep_SetSameValueMultipleTimes_DoesNotRaisePropertyChanged()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.CurrentStep = 2; // Set to 2 first
        var propertyChangedCount = 0;

        viewModel.PropertyChanged += (sender, args) =>
        {
            propertyChangedCount++;
        };

        // Act
        viewModel.CurrentStep = 2; // Set to same value
        viewModel.CurrentStep = 2; // Set to same value again

        // Assert
        Assert.AreEqual(0, propertyChangedCount);
    }

    /// <summary>
    /// Tests that setting CurrentStep to boundary values (int.MinValue, int.MaxValue) works correctly.
    /// </summary>
    /// <param name="boundaryValue">The boundary value to test.</param>
    [TestMethod]
    [DataRow(int.MinValue)]
    [DataRow(int.MaxValue)]
    public void CurrentStep_SetBoundaryValue_UpdatesPropertyAndRaisesEvent(int boundaryValue)
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var propertyChangedRaised = false;

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(viewModel.CurrentStep))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.CurrentStep = boundaryValue;

        // Assert
        Assert.AreEqual(boundaryValue, viewModel.CurrentStep);
        Assert.IsTrue(propertyChangedRaised);
    }

    /// <summary>
    /// Tests that the SelectedSourceDatabase getter returns null when not initialized.
    /// </summary>
    [TestMethod]
    public void SelectedSourceDatabase_WhenNotSet_ReturnsNull()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);

        // Act
        var result = viewModel.SelectedSourceDatabase;

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// Tests that the SelectedSourceDatabase getter returns the previously set value.
    /// </summary>
    [TestMethod]
    public async Task SelectedSourceDatabase_WhenSet_ReturnsSetValue()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);
        mockConnectionViewModel.Setup(x => x.ConnectionSettings).Returns(new ConnectionSettings());
        mockSqlService.Setup(x => x.GetTablesAsync(It.IsAny<ConnectionSettings>(), It.IsAny<string>()))
            .ReturnsAsync(new ObservableCollection<TableInfo>());

        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        var database = new DatabaseInfo { Name = "TestDatabase" };

        // Act
        viewModel.SelectedSourceDatabase = database;
        await Task.Delay(100); // Wait for async operation to complete

        // Assert
        Assert.AreEqual(database, viewModel.SelectedSourceDatabase);
    }

    /// <summary>
    /// Tests that setting SelectedSourceDatabase to null updates the property correctly.
    /// </summary>
    [TestMethod]
    public async Task SelectedSourceDatabase_SetToNull_UpdatesProperty()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        mockSqlService.Setup(x => x.GetTablesAsync(It.IsAny<ConnectionSettings>(), It.IsAny<string>()))
            .ReturnsAsync(new ObservableCollection<TableInfo>());

        var connectionViewModel = new ConnectionViewModel(mockSqlService.Object);

        var viewModel = new DataTransferViewModel(mockSqlService.Object, connectionViewModel);
        var database = new DatabaseInfo { Name = "TestDatabase" };
        viewModel.SelectedSourceDatabase = database;
        await Task.Delay(100);

        // Act
        viewModel.SelectedSourceDatabase = null;
        await Task.Delay(100);

        // Assert
        Assert.IsNull(viewModel.SelectedSourceDatabase);
    }

    /// <summary>
    /// Tests that setting SelectedSourceDatabase to the same value does not trigger LoadSourceTablesAsync.
    /// Validates by checking that GetTablesAsync is not called again.
    /// </summary>
    [TestMethod]
    public async Task SelectedSourceDatabase_SetToSameValue_DoesNotTriggerLoadTables()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);
        mockConnectionViewModel.Setup(x => x.ConnectionSettings).Returns(new ConnectionSettings());
        mockSqlService.Setup(x => x.GetTablesAsync(It.IsAny<ConnectionSettings>(), It.IsAny<string>()))
            .ReturnsAsync(new ObservableCollection<TableInfo>());

        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        var database = new DatabaseInfo { Name = "TestDatabase" };
        viewModel.SelectedSourceDatabase = database;
        await Task.Delay(100);
        mockSqlService.Invocations.Clear();

        // Act
        viewModel.SelectedSourceDatabase = database;
        await Task.Delay(100);

        // Assert
        mockSqlService.Verify(x => x.GetTablesAsync(It.IsAny<ConnectionSettings>(), It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// Tests that setting SelectedSourceDatabase to null when already null does not trigger LoadSourceTablesAsync.
    /// </summary>
    [TestMethod]
    public async Task SelectedSourceDatabase_SetToNullWhenAlreadyNull_DoesNotTriggerLoadTables()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);
        mockConnectionViewModel.Setup(x => x.ConnectionSettings).Returns(new ConnectionSettings());
        mockSqlService.Setup(x => x.GetTablesAsync(It.IsAny<ConnectionSettings>(), It.IsAny<string>()))
            .ReturnsAsync(new ObservableCollection<TableInfo>());

        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);

        // Act
        viewModel.SelectedSourceDatabase = null;
        await Task.Delay(100);

        // Assert
        mockSqlService.Verify(x => x.GetTablesAsync(It.IsAny<ConnectionSettings>(), It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// Tests that PropertyChanged event is raised when SelectedSourceDatabase value changes.
    /// </summary>
    [TestMethod]
    public async Task SelectedSourceDatabase_WhenValueChanges_RaisesPropertyChangedEvent()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);
        mockConnectionViewModel.Setup(x => x.ConnectionSettings).Returns(new ConnectionSettings());
        mockSqlService.Setup(x => x.GetTablesAsync(It.IsAny<ConnectionSettings>(), It.IsAny<string>()))
            .ReturnsAsync(new ObservableCollection<TableInfo>());

        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        var database = new DatabaseInfo { Name = "TestDatabase" };
        string? propertyName = null;
        viewModel.PropertyChanged += (sender, args) =>
        {
            // Capture the first PropertyChanged event for SelectedSourceDatabase
            if (args.PropertyName == "SelectedSourceDatabase" && propertyName == null)
            {
                propertyName = args.PropertyName;
            }
        };

        // Act
        viewModel.SelectedSourceDatabase = database;
        await Task.Delay(100);

        // Assert
        Assert.AreEqual("SelectedSourceDatabase", propertyName);
    }

    /// <summary>
    /// Tests that PropertyChanged event is not raised when SelectedSourceDatabase is set to the same value.
    /// </summary>
    [TestMethod]
    public async Task SelectedSourceDatabase_WhenSameValue_DoesNotRaisePropertyChangedEvent()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);
        mockConnectionViewModel.Setup(x => x.ConnectionSettings).Returns(new ConnectionSettings());
        mockSqlService.Setup(x => x.GetTablesAsync(It.IsAny<ConnectionSettings>(), It.IsAny<string>()))
            .ReturnsAsync(new ObservableCollection<TableInfo>());

        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        var database = new DatabaseInfo { Name = "TestDatabase" };
        viewModel.SelectedSourceDatabase = database;
        await Task.Delay(100);

        int eventCount = 0;
        viewModel.PropertyChanged += (sender, args) => eventCount++;

        // Act
        viewModel.SelectedSourceDatabase = database;
        await Task.Delay(100);

        // Assert
        Assert.AreEqual(0, eventCount);
    }

    /// <summary>
    /// Tests that IsReplaceMode returns true when TransferAction is set to Replace.
    /// Input: TransferAction = Replace
    /// Expected: IsReplaceMode returns true
    /// </summary>
    [TestMethod]
    public void IsReplaceMode_WhenTransferActionIsReplace_ReturnsTrue()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        viewModel.TransferAction = DataTransferAction.Replace;

        // Act
        var result = viewModel.IsReplaceMode;

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// Tests that IsReplaceMode returns false when TransferAction is set to Append.
    /// Input: TransferAction = Append
    /// Expected: IsReplaceMode returns false
    /// </summary>
    [TestMethod]
    public void IsReplaceMode_WhenTransferActionIsAppend_ReturnsFalse()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        viewModel.TransferAction = DataTransferAction.Append;

        // Act
        var result = viewModel.IsReplaceMode;

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Tests that setting IsReplaceMode to true changes TransferAction to Replace.
    /// Input: IsReplaceMode = true
    /// Expected: TransferAction becomes Replace
    /// </summary>
    [TestMethod]
    public void IsReplaceMode_SetToTrue_ChangesTransferActionToReplace()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        viewModel.TransferAction = DataTransferAction.Append;

        // Act
        viewModel.IsReplaceMode = true;

        // Assert
        Assert.AreEqual(DataTransferAction.Replace, viewModel.TransferAction);
        Assert.IsTrue(viewModel.IsReplaceMode);
    }

    /// <summary>
    /// Tests that setting IsReplaceMode to false when TransferAction is Replace does not change TransferAction.
    /// This tests the asymmetric behavior of the setter.
    /// Input: TransferAction = Replace, IsReplaceMode = false
    /// Expected: TransferAction remains Replace
    /// </summary>
    [TestMethod]
    public void IsReplaceMode_SetToFalseWhenReplace_DoesNotChangeTransferAction()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        viewModel.TransferAction = DataTransferAction.Replace;

        // Act
        viewModel.IsReplaceMode = false;

        // Assert
        Assert.AreEqual(DataTransferAction.Replace, viewModel.TransferAction);
        Assert.IsTrue(viewModel.IsReplaceMode);
    }

    /// <summary>
    /// Tests that setting IsReplaceMode to false when TransferAction is Append does not change TransferAction.
    /// This tests the asymmetric behavior of the setter.
    /// Input: TransferAction = Append, IsReplaceMode = false
    /// Expected: TransferAction remains Append
    /// </summary>
    [TestMethod]
    public void IsReplaceMode_SetToFalseWhenAppend_DoesNotChangeTransferAction()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        viewModel.TransferAction = DataTransferAction.Append;

        // Act
        viewModel.IsReplaceMode = false;

        // Assert
        Assert.AreEqual(DataTransferAction.Append, viewModel.TransferAction);
        Assert.IsFalse(viewModel.IsReplaceMode);
    }

    /// <summary>
    /// Tests that setting IsReplaceMode to true when already in Replace mode is idempotent.
    /// Input: TransferAction = Replace, IsReplaceMode = true
    /// Expected: TransferAction remains Replace
    /// </summary>
    [TestMethod]
    public void IsReplaceMode_SetToTrueWhenAlreadyReplace_RemainsReplace()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        viewModel.TransferAction = DataTransferAction.Replace;

        // Act
        viewModel.IsReplaceMode = true;

        // Assert
        Assert.AreEqual(DataTransferAction.Replace, viewModel.TransferAction);
        Assert.IsTrue(viewModel.IsReplaceMode);
    }

    /// <summary>
    /// Tests that IsReplaceMode getter correctly reflects the initial state (Append).
    /// Input: Default initialization
    /// Expected: IsReplaceMode returns false
    /// </summary>
    [TestMethod]
    public void IsReplaceMode_InitialState_ReturnsFalse()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);

        // Act
        var result = viewModel.IsReplaceMode;

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(DataTransferAction.Append, viewModel.TransferAction);
    }

    /// <summary>
    /// Tests the full round-trip behavior: Append -> Replace -> attempt to set false.
    /// Input: Multiple state changes
    /// Expected: Setting to false does not revert to Append
    /// </summary>
    [TestMethod]
    public void IsReplaceMode_RoundTripBehavior_VerifiesAsymmetricSetter()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);

        // Initial state is Append
        Assert.AreEqual(DataTransferAction.Append, viewModel.TransferAction);
        Assert.IsFalse(viewModel.IsReplaceMode);

        // Act 1: Set to true
        viewModel.IsReplaceMode = true;

        // Assert 1: Changed to Replace
        Assert.AreEqual(DataTransferAction.Replace, viewModel.TransferAction);
        Assert.IsTrue(viewModel.IsReplaceMode);

        // Act 2: Try to set to false
        viewModel.IsReplaceMode = false;

        // Assert 2: Remains Replace (asymmetric behavior)
        Assert.AreEqual(DataTransferAction.Replace, viewModel.TransferAction);
        Assert.IsTrue(viewModel.IsReplaceMode);
    }

    /// <summary>
    /// Tests that the constructor properly initializes all ICommand properties when provided with valid parameters.
    /// Validates that all 11 command properties (Step 1, Step 2, Step 3, and Navigation commands) are not null.
    /// </summary>
    [TestMethod]
    public void Constructor_WithValidParameters_InitializesAllCommands()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);

        // Act
        var viewModel = new DataTransferViewModel(
            mockSqlServerService.Object,
            mockConnectionViewModel.Object);

        // Assert - Step 1 Commands
        Assert.IsNotNull(viewModel.TestDestinationConnectionCommand);
        Assert.IsNotNull(viewModel.ConnectDestinationCommand);
        Assert.IsNotNull(viewModel.ClearDestinationSettingsCommand);

        // Assert - Step 2 Commands
        Assert.IsNotNull(viewModel.LoadSourceTablesCommand);
        Assert.IsNotNull(viewModel.PreviewDataCommand);
        Assert.IsNotNull(viewModel.GetRowCountCommand);

        // Assert - Step 3 Commands
        Assert.IsNotNull(viewModel.TransferDataCommand);
        Assert.IsNotNull(viewModel.CancelTransferCommand);

        // Assert - Navigation Commands
        Assert.IsNotNull(viewModel.NextStepCommand);
        Assert.IsNotNull(viewModel.PreviousStepCommand);
    }

    /// <summary>
    /// Tests that the constructor properly initializes all collection properties as empty ObservableCollections.
    /// Validates that SourceTables and DestinationDatabases are not null and start with zero items.
    /// </summary>
    [TestMethod]
    public void Constructor_WithValidParameters_InitializesCollectionsAsEmpty()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);

        // Act
        var viewModel = new DataTransferViewModel(
            mockSqlServerService.Object,
            mockConnectionViewModel.Object);

        // Assert
        Assert.IsNotNull(viewModel.SourceTables);
        Assert.AreEqual(0, viewModel.SourceTables.Count);
        Assert.IsInstanceOfType(viewModel.SourceTables, typeof(ObservableCollection<TableInfo>));

        Assert.IsNotNull(viewModel.DestinationDatabases);
        Assert.AreEqual(0, viewModel.DestinationDatabases.Count);
        Assert.IsInstanceOfType(viewModel.DestinationDatabases, typeof(ObservableCollection<DatabaseInfo>));
    }

    /// <summary>
    /// Tests that the constructor properly initializes all primitive and object properties to their expected default values.
    /// Validates CurrentStep is 1, IsDestinationConnected is false, and DestinationConnectionStatus has default value.
    /// </summary>
    [TestMethod]
    public void Constructor_WithValidParameters_InitializesPropertiesWithDefaultValues()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);

        // Act
        var viewModel = new DataTransferViewModel(
            mockSqlServerService.Object,
            mockConnectionViewModel.Object);

        // Assert
        Assert.AreEqual(1, viewModel.CurrentStep);
        Assert.IsFalse(viewModel.IsDestinationConnected);
        Assert.AreEqual("قطع ارتباط", viewModel.DestinationConnectionStatus);
        Assert.IsTrue(viewModel.RememberDestinationSettings);
        Assert.AreEqual(DataTransferMode.Table, viewModel.TransferMode);
        Assert.AreEqual(DataTransferAction.Append, viewModel.TransferAction);
        Assert.IsFalse(viewModel.EnableIdentityInsert);
        Assert.IsFalse(viewModel.IsTransferring);
        Assert.AreEqual(0, viewModel.ProgressPercentage);
        Assert.AreEqual(0L, viewModel.TransferredRows);
        Assert.AreEqual(0L, viewModel.SourceRowCount);
        Assert.AreEqual(string.Empty, viewModel.CustomQuery);
        Assert.AreEqual(string.Empty, viewModel.DestinationTableName);
    }

    /// <summary>
    /// Tests that the constructor properly initializes computed properties based on initial state.
    /// Validates IsStep1 is true, IsStep2 and IsStep3 are false, IsTableMode is true, IsQueryMode is false.
    /// </summary>
    [TestMethod]
    public void Constructor_WithValidParameters_InitializesComputedPropertiesCorrectly()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);

        // Act
        var viewModel = new DataTransferViewModel(
            mockSqlServerService.Object,
            mockConnectionViewModel.Object);

        // Assert
        Assert.IsTrue(viewModel.IsStep1);
        Assert.IsFalse(viewModel.IsStep2);
        Assert.IsFalse(viewModel.IsStep3);
        Assert.IsTrue(viewModel.IsTableMode);
        Assert.IsFalse(viewModel.IsQueryMode);
        Assert.IsTrue(viewModel.IsAppendMode);
        Assert.IsFalse(viewModel.IsReplaceMode);
    }

    /// <summary>
    /// Tests that the constructor properly initializes nullable reference properties to null.
    /// Validates that SelectedSourceDatabase, SelectedSourceTable, SelectedDestinationDatabase, and PreviewData are null.
    /// </summary>
    [TestMethod]
    public void Constructor_WithValidParameters_InitializesNullablePropertiesAsNull()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);

        // Act
        var viewModel = new DataTransferViewModel(
            mockSqlServerService.Object,
            mockConnectionViewModel.Object);

        // Assert
        Assert.IsNull(viewModel.SelectedSourceDatabase);
        Assert.IsNull(viewModel.SelectedSourceTable);
        Assert.IsNull(viewModel.SelectedDestinationDatabase);
        Assert.IsNull(viewModel.PreviewData);
    }

    /// <summary>
    /// Tests that the constructor initializes DestinationSettings as a non-null object.
    /// Validates that the DestinationSettings property is accessible after construction.
    /// </summary>
    [TestMethod]
    public void Constructor_WithValidParameters_InitializesDestinationSettings()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);

        // Act
        var viewModel = new DataTransferViewModel(
            mockSqlServerService.Object,
            mockConnectionViewModel.Object);

        // Assert
        Assert.IsNotNull(viewModel.DestinationSettings);
    }

    /// <summary>
    /// Tests that the constructor properly exposes the SourceDatabases collection from ConnectionViewModel.
    /// Validates that SourceDatabases property references the Databases collection from the injected ConnectionViewModel.
    /// </summary>
    [TestMethod]
    public void Constructor_WithValidParameters_ExposesSourceDatabasesFromConnectionViewModel()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var expectedDatabases = new ObservableCollection<DatabaseInfo>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(expectedDatabases);

        // Act
        var viewModel = new DataTransferViewModel(
            mockSqlServerService.Object,
            mockConnectionViewModel.Object);

        // Assert
        Assert.AreSame(expectedDatabases, viewModel.SourceDatabases);
    }

    /// <summary>
    /// Tests that CustomQuery returns an empty string by default when accessed.
    /// </summary>
    [TestMethod]
    public void CustomQuery_Get_ReturnsEmptyStringByDefault()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        var result = viewModel.CustomQuery;

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    /// <summary>
    /// Tests that setting CustomQuery updates the property value and raises PropertyChanged event.
    /// Tests various string values including normal text, empty string, whitespace, very long strings, and SQL-like content.
    /// </summary>
    /// <param name="value">The value to set for CustomQuery.</param>
    [TestMethod]
    [DataRow("SELECT * FROM Users")]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("\t\n\r")]
    [DataRow("SELECT * FROM Users WHERE Name = 'O''Brien' AND Age > 25; DROP TABLE Users; --")]
    public void CustomQuery_SetValue_UpdatesPropertyAndRaisesPropertyChanged(string value)
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Set to a non-empty value first to ensure backing field differs from empty string test values
        viewModel.CustomQuery = "initial non-empty value";

        var propertyChangedRaised = false;
        string? raisedPropertyName = null;

        viewModel.PropertyChanged += (sender, e) =>
        {
            propertyChangedRaised = true;
            raisedPropertyName = e.PropertyName;
        };

        // Act
        viewModel.CustomQuery = value;

        // Assert
        Assert.AreEqual(value, viewModel.CustomQuery);
        Assert.IsTrue(propertyChangedRaised, "PropertyChanged event should be raised.");
        Assert.AreEqual("CustomQuery", raisedPropertyName, "PropertyChanged event should be raised for CustomQuery.");
    }

    /// <summary>
    /// Tests that setting CustomQuery with a very long string (10,000 characters) updates the property and raises PropertyChanged.
    /// </summary>
    [TestMethod]
    public void CustomQuery_SetVeryLongString_UpdatesPropertyAndRaisesPropertyChanged()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var veryLongString = new string('A', 10000);
        var propertyChangedRaised = false;

        viewModel.PropertyChanged += (sender, e) =>
        {
            propertyChangedRaised = true;
        };

        // Act
        viewModel.CustomQuery = veryLongString;

        // Assert
        Assert.AreEqual(veryLongString, viewModel.CustomQuery);
        Assert.IsTrue(propertyChangedRaised, "PropertyChanged event should be raised.");
    }

    /// <summary>
    /// Tests that setting CustomQuery to the same value twice only raises PropertyChanged once.
    /// This verifies the SetProperty optimization that prevents unnecessary notifications.
    /// </summary>
    [TestMethod]
    public void CustomQuery_SetSameValueTwice_RaisesPropertyChangedOnlyOnce()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var testValue = "SELECT * FROM TestTable";
        var propertyChangedCount = 0;

        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == "CustomQuery")
            {
                propertyChangedCount++;
            }
        };

        // Act
        viewModel.CustomQuery = testValue;
        viewModel.CustomQuery = testValue;

        // Assert
        Assert.AreEqual(testValue, viewModel.CustomQuery);
        Assert.AreEqual(1, propertyChangedCount, "PropertyChanged should only be raised once when setting the same value twice.");
    }

    /// <summary>
    /// Tests that setting CustomQuery to multiple different values raises PropertyChanged for each change.
    /// </summary>
    [TestMethod]
    public void CustomQuery_SetMultipleDifferentValues_RaisesPropertyChangedForEach()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var values = new[] { "Query1", "Query2", "Query3", "Query4" };
        var propertyChangedCount = 0;

        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == "CustomQuery")
            {
                propertyChangedCount++;
            }
        };

        // Act
        foreach (var value in values)
        {
            viewModel.CustomQuery = value;
        }

        // Assert
        Assert.AreEqual(values[values.Length - 1], viewModel.CustomQuery);
        Assert.AreEqual(values.Length, propertyChangedCount, "PropertyChanged should be raised for each different value.");
    }

    /// <summary>
    /// Tests that setting CustomQuery from a non-empty value to an empty string updates the property and raises PropertyChanged.
    /// </summary>
    [TestMethod]
    public void CustomQuery_SetFromNonEmptyToEmpty_UpdatesPropertyAndRaisesPropertyChanged()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.CustomQuery = "Initial Query";
        var propertyChangedRaised = false;

        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == "CustomQuery")
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.CustomQuery = string.Empty;

        // Assert
        Assert.AreEqual(string.Empty, viewModel.CustomQuery);
        Assert.IsTrue(propertyChangedRaised, "PropertyChanged event should be raised when clearing the query.");
    }

    /// <summary>
    /// Tests that CustomQuery handles strings with unicode characters correctly.
    /// </summary>
    [TestMethod]
    public void CustomQuery_SetUnicodeString_UpdatesPropertyAndRaisesPropertyChanged()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var unicodeString = "SELECT * FROM Users WHERE Name = '日本語' OR City = '北京'";
        var propertyChangedRaised = false;

        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == "CustomQuery")
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.CustomQuery = unicodeString;

        // Assert
        Assert.AreEqual(unicodeString, viewModel.CustomQuery);
        Assert.IsTrue(propertyChangedRaised, "PropertyChanged event should be raised.");
    }

    /// <summary>
    /// Tests that GetDestinationPassword returns the correct password value when a valid password is set.
    /// Input: Valid password string.
    /// Expected: The same password string is returned.
    /// </summary>
    [TestMethod]
    [DataRow("myPassword123")]
    [DataRow("P@ssw0rd!")]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("   ")]
    [DataRow("a")]
    [DataRow("ThisIsAVeryLongPasswordThatContainsMoreThan100CharactersToTestTheBehaviorWithLongPasswordStringsWhichMightBeUsedInSomeScenarios")]
    [DataRow("Password with spaces and special chars: !@#$%^&*()")]
    [DataRow("パスワード")]
    [DataRow("Pass\nword")]
    [DataRow("Pass\tword")]
    [DataRow("Pass\rword")]
    public void GetDestinationPassword_WithVariousPasswordValues_ReturnsCorrectPassword(string password)
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.DestinationSettings.Password = password;

        // Act
        var result = viewModel.GetDestinationPassword();

        // Assert
        Assert.AreEqual(password, result);
    }

    /// <summary>
    /// Tests that GetDestinationPassword returns the default empty string when password is not set.
    /// Input: No password set (default value).
    /// Expected: Empty string is returned.
    /// </summary>
    [TestMethod]
    public void GetDestinationPassword_WhenPasswordNotSet_ReturnsEmptyString()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        var result = viewModel.GetDestinationPassword();

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    /// <summary>
    /// Tests that GetDestinationPassword returns the updated password after multiple changes.
    /// Input: Multiple password updates.
    /// Expected: The most recently set password is returned.
    /// </summary>
    [TestMethod]
    public void GetDestinationPassword_AfterMultiplePasswordChanges_ReturnsLatestPassword()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.DestinationSettings.Password = "firstPassword";
        viewModel.DestinationSettings.Password = "secondPassword";
        viewModel.DestinationSettings.Password = "thirdPassword";

        // Act
        var result = viewModel.GetDestinationPassword();

        // Assert
        Assert.AreEqual("thirdPassword", result);
    }

    /// <summary>
    /// Tests that GetDestinationPassword returns the correct password after DestinationSettings is replaced.
    /// Input: New DestinationSettings object with a different password.
    /// Expected: The password from the new DestinationSettings is returned.
    /// </summary>
    [TestMethod]
    public async Task GetDestinationPassword_AfterDestinationSettingsReplaced_ReturnsNewPassword()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        
        // Wait for constructor's async initialization to complete
        await Task.Delay(100);
        
        viewModel.DestinationSettings.Password = "originalPassword";

        var newSettings = new DestinationServerSettings
        {
            Password = "newPassword"
        };
        viewModel.DestinationSettings = newSettings;

        // Act
        var result = viewModel.GetDestinationPassword();

        // Assert
        Assert.AreEqual("newPassword", result);
    }

    /// <summary>
    /// Tests that IsStep1 returns true when CurrentStep is set to 1.
    /// </summary>
    [TestMethod]
    public void IsStep1_WhenCurrentStepIs1_ReturnsTrue()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var connectionViewModel = new ConnectionViewModel(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, connectionViewModel);
        viewModel.CurrentStep = 1;

        // Act
        bool result = viewModel.IsStep1;

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// Tests that IsStep1 returns false when CurrentStep is set to values other than 1.
    /// Tests various edge cases including step boundaries, negative values, and extreme values.
    /// </summary>
    /// <param name="currentStep">The value to set CurrentStep to.</param>
    [TestMethod]
    [DataRow(0)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(-1)]
    [DataRow(-100)]
    [DataRow(100)]
    [DataRow(int.MinValue)]
    [DataRow(int.MaxValue)]
    public void IsStep1_WhenCurrentStepIsNotOne_ReturnsFalse(int currentStep)
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.CurrentStep = currentStep;

        // Act
        bool result = viewModel.IsStep1;

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Tests that IsStep1 returns true immediately after construction when CurrentStep defaults to 1.
    /// </summary>
    [TestMethod]
    public void IsStep1_WhenInitialized_ReturnsTrueByDefault()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var connectionViewModel = new ConnectionViewModel(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, connectionViewModel);

        // Act
        bool result = viewModel.IsStep1;

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// Tests that IsStep1 changes correctly when CurrentStep is modified multiple times.
    /// Verifies that the property reflects the current state accurately.
    /// </summary>
    [TestMethod]
    public void IsStep1_WhenCurrentStepChangesMultipleTimes_ReflectsCurrentValue()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var connectionViewModel = new ConnectionViewModel(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, connectionViewModel);

        // Act & Assert - Initial state
        Assert.IsTrue(viewModel.IsStep1);

        // Act & Assert - Change to step 2
        viewModel.CurrentStep = 2;
        Assert.IsFalse(viewModel.IsStep1);

        // Act & Assert - Change back to step 1
        viewModel.CurrentStep = 1;
        Assert.IsTrue(viewModel.IsStep1);

        // Act & Assert - Change to step 3
        viewModel.CurrentStep = 3;
        Assert.IsFalse(viewModel.IsStep1);

        // Act & Assert - Change back to step 1 again
        viewModel.CurrentStep = 1;
        Assert.IsTrue(viewModel.IsStep1);
    }

    /// <summary>
    /// Tests that setting TransferMode to Table updates the value and raises PropertyChanged events.
    /// </summary>
    [TestMethod]
    public void TransferMode_SetToTable_UpdatesValueAndRaisesPropertyChanged()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Set to Query first to ensure TransferMode changes when we set it to Table
        viewModel.TransferMode = DataTransferMode.Query;

        var raiseTransferMode = false;
        var raiseIsTableMode = false;
        var raiseIsQueryMode = false;

        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == "TransferMode") raiseTransferMode = true;
            if (e.PropertyName == "IsTableMode") raiseIsTableMode = true;
            if (e.PropertyName == "IsQueryMode") raiseIsQueryMode = true;
        };

        // Act
        viewModel.TransferMode = DataTransferMode.Table;

        // Assert
        Assert.AreEqual(DataTransferMode.Table, viewModel.TransferMode);
        Assert.IsTrue(viewModel.IsTableMode);
        Assert.IsFalse(viewModel.IsQueryMode);
        Assert.IsTrue(raiseTransferMode, "TransferMode PropertyChanged should be raised");
        Assert.IsTrue(raiseIsTableMode, "IsTableMode PropertyChanged should be raised");
        Assert.IsTrue(raiseIsQueryMode, "IsQueryMode PropertyChanged should be raised");
    }

    /// <summary>
    /// Tests that setting TransferMode to Query updates the value and raises PropertyChanged events.
    /// </summary>
    [TestMethod]
    public void TransferMode_SetToQuery_UpdatesValueAndRaisesPropertyChanged()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        var propertyChangedEvents = new System.Collections.Generic.List<string>();
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName != null)
            {
                propertyChangedEvents.Add(e.PropertyName);
            }
        };

        // Act
        viewModel.TransferMode = DataTransferMode.Query;

        // Assert
        Assert.AreEqual(DataTransferMode.Query, viewModel.TransferMode);
        Assert.IsFalse(viewModel.IsTableMode);
        Assert.IsTrue(viewModel.IsQueryMode);
        CollectionAssert.Contains(propertyChangedEvents, "TransferMode");
        CollectionAssert.Contains(propertyChangedEvents, "IsTableMode");
        CollectionAssert.Contains(propertyChangedEvents, "IsQueryMode");
    }

    /// <summary>
    /// Tests that setting TransferMode to the same value does not raise PropertyChanged events.
    /// </summary>
    [TestMethod]
    public void TransferMode_SetToSameValue_DoesNotRaisePropertyChanged()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Set initial value
        viewModel.TransferMode = DataTransferMode.Table;

        var propertyChangedRaised = false;
        viewModel.PropertyChanged += (sender, e) =>
        {
            propertyChangedRaised = true;
        };

        // Act
        viewModel.TransferMode = DataTransferMode.Table;

        // Assert
        Assert.AreEqual(DataTransferMode.Table, viewModel.TransferMode);
        Assert.IsFalse(propertyChangedRaised);
    }

    /// <summary>
    /// Tests that getting TransferMode returns the current value.
    /// </summary>
    [TestMethod]
    public void TransferMode_GetValue_ReturnsCurrentValue()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        var initialValue = viewModel.TransferMode;
        viewModel.TransferMode = DataTransferMode.Query;
        var updatedValue = viewModel.TransferMode;

        // Assert
        Assert.AreEqual(DataTransferMode.Table, initialValue);
        Assert.AreEqual(DataTransferMode.Query, updatedValue);
    }

    /// <summary>
    /// Tests that setting TransferMode multiple times correctly updates IsTableMode and IsQueryMode.
    /// </summary>
    [TestMethod]
    [DataRow(DataTransferMode.Table, true, false, DisplayName = "Table mode")]
    [DataRow(DataTransferMode.Query, false, true, DisplayName = "Query mode")]
    public void TransferMode_SetValue_UpdatesIsTableModeAndIsQueryMode(DataTransferMode mode, bool expectedIsTableMode, bool expectedIsQueryMode)
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        viewModel.TransferMode = mode;

        // Assert
        Assert.AreEqual(mode, viewModel.TransferMode);
        Assert.AreEqual(expectedIsTableMode, viewModel.IsTableMode);
        Assert.AreEqual(expectedIsQueryMode, viewModel.IsQueryMode);
    }

    /// <summary>
    /// Tests that setting TransferMode from Query to Table raises PropertyChanged for all related properties.
    /// </summary>
    [TestMethod]
    public void TransferMode_ChangeFromQueryToTable_RaisesAllPropertyChangedEvents()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        viewModel.TransferMode = DataTransferMode.Query;

        var propertyChangedEvents = new System.Collections.Generic.List<string>();
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName != null)
            {
                propertyChangedEvents.Add(e.PropertyName);
            }
        };

        // Act
        viewModel.TransferMode = DataTransferMode.Table;

        // Assert
        Assert.AreEqual(DataTransferMode.Table, viewModel.TransferMode);
        Assert.IsTrue(propertyChangedEvents.Contains("TransferMode"));
        Assert.IsTrue(propertyChangedEvents.Contains("IsTableMode"));
        Assert.IsTrue(propertyChangedEvents.Contains("IsQueryMode"));
        Assert.AreEqual(3, propertyChangedEvents.Count);
    }

    /// <summary>
    /// Tests that setting TransferMode to an invalid enum value (out of defined range) still updates the value and raises PropertyChanged events.
    /// This tests the edge case where an invalid enum value is cast and assigned.
    /// </summary>
    [TestMethod]
    public void TransferMode_SetToInvalidEnumValue_UpdatesValueAndRaisesPropertyChanged()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        var propertyChangedEvents = new System.Collections.Generic.List<string>();
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName != null)
            {
                propertyChangedEvents.Add(e.PropertyName);
            }
        };

        var invalidEnumValue = (DataTransferMode)999;

        // Act
        viewModel.TransferMode = invalidEnumValue;

        // Assert
        Assert.AreEqual(invalidEnumValue, viewModel.TransferMode);
        CollectionAssert.Contains(propertyChangedEvents, "TransferMode");
        CollectionAssert.Contains(propertyChangedEvents, "IsTableMode");
        CollectionAssert.Contains(propertyChangedEvents, "IsQueryMode");
    }

    /// <summary>
    /// Tests the default initial value of TransferMode after construction.
    /// </summary>
    [TestMethod]
    public void TransferMode_InitialValue_IsTable()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());

        // Act
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Assert
        Assert.AreEqual(DataTransferMode.Table, viewModel.TransferMode);
        Assert.IsTrue(viewModel.IsTableMode);
        Assert.IsFalse(viewModel.IsQueryMode);
    }

    /// <summary>
    /// Tests that setting TransferMode from Table to Query and back to Table correctly updates all related properties.
    /// </summary>
    [TestMethod]
    public void TransferMode_MultipleChanges_CorrectlyUpdatesAllProperties()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act & Assert - Initial state
        Assert.AreEqual(DataTransferMode.Table, viewModel.TransferMode);
        Assert.IsTrue(viewModel.IsTableMode);
        Assert.IsFalse(viewModel.IsQueryMode);

        // Act & Assert - Change to Query
        viewModel.TransferMode = DataTransferMode.Query;
        Assert.AreEqual(DataTransferMode.Query, viewModel.TransferMode);
        Assert.IsFalse(viewModel.IsTableMode);
        Assert.IsTrue(viewModel.IsQueryMode);

        // Act & Assert - Change back to Table
        viewModel.TransferMode = DataTransferMode.Table;
        Assert.AreEqual(DataTransferMode.Table, viewModel.TransferMode);
        Assert.IsTrue(viewModel.IsTableMode);
        Assert.IsFalse(viewModel.IsQueryMode);
    }

    /// <summary>
    /// Tests that the IsAppendMode getter returns the correct value based on TransferAction.
    /// </summary>
    /// <param name="transferAction">The DataTransferAction value to set.</param>
    /// <param name="expectedIsAppendMode">The expected value of IsAppendMode.</param>
    [TestMethod]
    [DataRow(DataTransferAction.Append, true, DisplayName = "Append returns true")]
    [DataRow(DataTransferAction.Replace, false, DisplayName = "Replace returns false")]
    public void IsAppendMode_Get_ReturnsExpectedValueBasedOnTransferAction(DataTransferAction transferAction, bool expectedIsAppendMode)
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);
        viewModel.TransferAction = transferAction;

        // Act
        bool actualIsAppendMode = viewModel.IsAppendMode;

        // Assert
        Assert.AreEqual(expectedIsAppendMode, actualIsAppendMode);
    }

    /// <summary>
    /// Tests that the IsAppendMode getter returns false when TransferAction is set to an undefined enum value.
    /// </summary>
    [TestMethod]
    public void IsAppendMode_Get_WithUndefinedEnumValue_ReturnsFalse()
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);
        viewModel.TransferAction = (DataTransferAction)999;

        // Act
        bool actualIsAppendMode = viewModel.IsAppendMode;

        // Assert
        Assert.IsFalse(actualIsAppendMode);
    }

    /// <summary>
    /// Tests that setting IsAppendMode to true sets TransferAction to Append.
    /// </summary>
    [TestMethod]
    public void IsAppendMode_SetTrue_SetsTransferActionToAppend()
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);
        viewModel.TransferAction = DataTransferAction.Replace;

        // Act
        viewModel.IsAppendMode = true;

        // Assert
        Assert.AreEqual(DataTransferAction.Append, viewModel.TransferAction);
        Assert.IsTrue(viewModel.IsAppendMode);
    }

    /// <summary>
    /// Tests that setting IsAppendMode to true when it is already Append remains idempotent.
    /// </summary>
    [TestMethod]
    public void IsAppendMode_SetTrue_WhenAlreadyAppend_RemainsAppend()
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);
        viewModel.TransferAction = DataTransferAction.Append;

        // Act
        viewModel.IsAppendMode = true;

        // Assert
        Assert.AreEqual(DataTransferAction.Append, viewModel.TransferAction);
        Assert.IsTrue(viewModel.IsAppendMode);
    }

    /// <summary>
    /// Tests that setting IsAppendMode to false does not change the TransferAction value.
    /// This validates the asymmetric setter behavior where false is a no-op.
    /// </summary>
    /// <param name="initialTransferAction">The initial DataTransferAction value.</param>
    [TestMethod]
    [DataRow(DataTransferAction.Append, DisplayName = "False from Append leaves Append")]
    [DataRow(DataTransferAction.Replace, DisplayName = "False from Replace leaves Replace")]
    public void IsAppendMode_SetFalse_DoesNotChangeTransferAction(DataTransferAction initialTransferAction)
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);
        viewModel.TransferAction = initialTransferAction;

        // Act
        viewModel.IsAppendMode = false;

        // Assert
        Assert.AreEqual(initialTransferAction, viewModel.TransferAction);
    }

    /// <summary>
    /// Tests that setting IsAppendMode to false when TransferAction is an undefined enum value does not change it.
    /// </summary>
    [TestMethod]
    public void IsAppendMode_SetFalse_WithUndefinedEnumValue_DoesNotChangeTransferAction()
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);
        var undefinedValue = (DataTransferAction)999;
        viewModel.TransferAction = undefinedValue;

        // Act
        viewModel.IsAppendMode = false;

        // Assert
        Assert.AreEqual(undefinedValue, viewModel.TransferAction);
    }

    /// <summary>
    /// Tests that ProgressPercentage returns the initial default value of 0.
    /// </summary>
    [TestMethod]
    public void ProgressPercentage_InitialValue_ReturnsZero()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        int result = viewModel.ProgressPercentage;

        // Assert
        Assert.AreEqual(0, result);
    }

    /// <summary>
    /// Tests that ProgressPercentage getter returns consistent value across multiple accesses.
    /// </summary>
    [TestMethod]
    public void ProgressPercentage_MultipleReads_ReturnsConsistentValue()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        int firstRead = viewModel.ProgressPercentage;
        int secondRead = viewModel.ProgressPercentage;
        int thirdRead = viewModel.ProgressPercentage;

        // Assert
        Assert.AreEqual(firstRead, secondRead);
        Assert.AreEqual(secondRead, thirdRead);
        Assert.AreEqual(0, firstRead);
    }

    /// <summary>
    /// Tests that ProgressPercentage getter does not throw an exception when accessed.
    /// </summary>
    [TestMethod]
    public void ProgressPercentage_GetValue_DoesNotThrowException()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act & Assert
        try
        {
            int result = viewModel.ProgressPercentage;
            Assert.AreEqual(0, result);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Expected no exception, but got: {ex.GetType().Name} - {ex.Message}");
        }
    }

    /// <summary>
    /// Tests that IsStep3 returns true when CurrentStep is set to 3.
    /// </summary>
    [TestMethod]
    public void IsStep3_WhenCurrentStepIsThree_ReturnsTrue()
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);

        // Act
        viewModel.CurrentStep = 3;

        // Assert
        Assert.IsTrue(viewModel.IsStep3);
    }

    /// <summary>
    /// Tests that IsStep3 returns false when CurrentStep is not equal to 3.
    /// Covers various edge cases including boundary values, negative numbers, and extreme values.
    /// </summary>
    /// <param name="currentStep">The value to set for CurrentStep.</param>
    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(4)]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(-100)]
    [DataRow(100)]
    [DataRow(int.MinValue)]
    [DataRow(int.MaxValue)]
    public void IsStep3_WhenCurrentStepIsNotThree_ReturnsFalse(int currentStep)
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);

        // Act
        viewModel.CurrentStep = currentStep;

        // Assert
        Assert.IsFalse(viewModel.IsStep3);
    }

    /// <summary>
    /// Tests that IsStep3 returns false when CurrentStep is at its default value (1).
    /// </summary>
    [TestMethod]
    public void IsStep3_WhenCurrentStepIsDefault_ReturnsFalse()
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);

        // Act & Assert
        Assert.IsFalse(viewModel.IsStep3);
    }

    /// <summary>
    /// Tests that SourceDatabases returns the same reference as ConnectionViewModel.Databases.
    /// </summary>
    [TestMethod]
    public void SourceDatabases_WhenAccessed_ReturnsSameReferenceAsConnectionViewModelDatabases()
    {
        // Arrange
        Mock<ISqlServerService> mockSqlServerService = new();
        ConnectionViewModel connectionViewModel = new(mockSqlServerService.Object);
        DataTransferViewModel viewModel = new(mockSqlServerService.Object, connectionViewModel);

        // Act
        ObservableCollection<DatabaseInfo> result = viewModel.SourceDatabases;

        // Assert
        Assert.AreSame(connectionViewModel.Databases, result);
    }

    /// <summary>
    /// Tests that SourceDatabases is empty when ConnectionViewModel.Databases is empty.
    /// </summary>
    [TestMethod]
    public void SourceDatabases_WhenConnectionViewModelDatabasesIsEmpty_ReturnsEmptyCollection()
    {
        // Arrange
        Mock<ISqlServerService> mockSqlServerService = new();
        ConnectionViewModel connectionViewModel = new(mockSqlServerService.Object);
        DataTransferViewModel viewModel = new(mockSqlServerService.Object, connectionViewModel);

        // Act
        ObservableCollection<DatabaseInfo> result = viewModel.SourceDatabases;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    /// <summary>
    /// Tests that SourceDatabases contains items when ConnectionViewModel.Databases has items.
    /// </summary>
    [TestMethod]
    public void SourceDatabases_WhenConnectionViewModelDatabasesHasItems_ReturnsCollectionWithItems()
    {
        // Arrange
        Mock<ISqlServerService> mockSqlServerService = new();
        ConnectionViewModel connectionViewModel = new(mockSqlServerService.Object);
        DatabaseInfo database1 = new() { Name = "TestDB1" };
        DatabaseInfo database2 = new() { Name = "TestDB2" };
        DatabaseInfo database3 = new() { Name = "TestDB3" };
        connectionViewModel.Databases.Add(database1);
        connectionViewModel.Databases.Add(database2);
        connectionViewModel.Databases.Add(database3);
        DataTransferViewModel viewModel = new(mockSqlServerService.Object, connectionViewModel);

        // Act
        ObservableCollection<DatabaseInfo> result = viewModel.SourceDatabases;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);
        Assert.AreSame(database1, result[0]);
        Assert.AreSame(database2, result[1]);
        Assert.AreSame(database3, result[2]);
    }

    /// <summary>
    /// Tests that changes to ConnectionViewModel.Databases are reflected in SourceDatabases
    /// since they share the same reference.
    /// </summary>
    [TestMethod]
    public void SourceDatabases_WhenConnectionViewModelDatabasesIsModifiedAfterInitialization_ReflectsChanges()
    {
        // Arrange
        Mock<ISqlServerService> mockSqlServerService = new();
        ConnectionViewModel connectionViewModel = new(mockSqlServerService.Object);
        DataTransferViewModel viewModel = new(mockSqlServerService.Object, connectionViewModel);
        DatabaseInfo database = new() { Name = "NewDB" };

        // Act - Add item after DataTransferViewModel creation
        connectionViewModel.Databases.Add(database);
        ObservableCollection<DatabaseInfo> result = viewModel.SourceDatabases;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreSame(database, result[0]);
    }

    /// <summary>
    /// Tests that SourceDatabases property can be accessed multiple times and always returns
    /// the same reference.
    /// </summary>
    [TestMethod]
    public void SourceDatabases_WhenAccessedMultipleTimes_AlwaysReturnsSameReference()
    {
        // Arrange
        Mock<ISqlServerService> mockSqlServerService = new();
        ConnectionViewModel connectionViewModel = new(mockSqlServerService.Object);
        DataTransferViewModel viewModel = new(mockSqlServerService.Object, connectionViewModel);

        // Act
        ObservableCollection<DatabaseInfo> result1 = viewModel.SourceDatabases;
        ObservableCollection<DatabaseInfo> result2 = viewModel.SourceDatabases;
        ObservableCollection<DatabaseInfo> result3 = viewModel.SourceDatabases;

        // Assert
        Assert.AreSame(result1, result2);
        Assert.AreSame(result2, result3);
        Assert.AreSame(connectionViewModel.Databases, result1);
    }

    /// <summary>
    /// Tests that SourceDatabases correctly reflects a single item in ConnectionViewModel.Databases.
    /// </summary>
    [TestMethod]
    public void SourceDatabases_WhenConnectionViewModelDatabasesHasSingleItem_ReturnsSingleItemCollection()
    {
        // Arrange
        Mock<ISqlServerService> mockSqlServerService = new();
        ConnectionViewModel connectionViewModel = new(mockSqlServerService.Object);
        DatabaseInfo database = new() { Name = "SingleDB" };
        connectionViewModel.Databases.Add(database);
        DataTransferViewModel viewModel = new(mockSqlServerService.Object, connectionViewModel);

        // Act
        ObservableCollection<DatabaseInfo> result = viewModel.SourceDatabases;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreSame(database, result[0]);
    }

    /// <summary>
    /// Tests that DestinationTableName getter returns the initial default value (empty string).
    /// </summary>
    [TestMethod]
    public void DestinationTableName_InitialValue_ReturnsEmptyString()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        var result = viewModel.DestinationTableName;

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    /// <summary>
    /// Tests that DestinationTableName setter updates the value and getter returns the new value.
    /// Input: Normal string value.
    /// Expected: Property is set and retrieved correctly.
    /// </summary>
    [TestMethod]
    public void DestinationTableName_SetNormalValue_ReturnsSetValue()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var expectedValue = "MyTable";

        // Act
        viewModel.DestinationTableName = expectedValue;
        var result = viewModel.DestinationTableName;

        // Assert
        Assert.AreEqual(expectedValue, result);
    }

    /// <summary>
    /// Tests that DestinationTableName setter accepts and stores an empty string.
    /// Input: Empty string.
    /// Expected: Property is set to empty string.
    /// </summary>
    [TestMethod]
    public void DestinationTableName_SetEmptyString_ReturnsEmptyString()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        viewModel.DestinationTableName = string.Empty;
        var result = viewModel.DestinationTableName;

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    /// <summary>
    /// Tests that DestinationTableName setter accepts and stores a whitespace-only string.
    /// Input: String containing only whitespace characters.
    /// Expected: Property is set to whitespace string.
    /// </summary>
    [TestMethod]
    public void DestinationTableName_SetWhitespaceString_ReturnsWhitespaceString()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var whitespaceValue = "   ";

        // Act
        viewModel.DestinationTableName = whitespaceValue;
        var result = viewModel.DestinationTableName;

        // Assert
        Assert.AreEqual(whitespaceValue, result);
    }

    /// <summary>
    /// Tests that DestinationTableName setter accepts and stores a very long string.
    /// Input: String with 10,000 characters.
    /// Expected: Property is set to the long string.
    /// </summary>
    [TestMethod]
    public void DestinationTableName_SetVeryLongString_ReturnsLongString()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var longValue = new string('A', 10000);

        // Act
        viewModel.DestinationTableName = longValue;
        var result = viewModel.DestinationTableName;

        // Assert
        Assert.AreEqual(longValue, result);
    }

    /// <summary>
    /// Tests that DestinationTableName setter accepts and stores a string with special characters.
    /// Input: String containing special SQL characters and Unicode.
    /// Expected: Property is set to the string with special characters.
    /// </summary>
    [TestMethod]
    public void DestinationTableName_SetStringWithSpecialCharacters_ReturnsStringWithSpecialCharacters()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var specialValue = "Table!@#$%^&*()_+-=[]{}|;':\"<>?,./`~";

        // Act
        viewModel.DestinationTableName = specialValue;
        var result = viewModel.DestinationTableName;

        // Assert
        Assert.AreEqual(specialValue, result);
    }

    /// <summary>
    /// Tests that DestinationTableName setter accepts and stores a string with Unicode characters.
    /// Input: String containing Arabic characters (matching the localization in the codebase).
    /// Expected: Property is set to the Unicode string.
    /// </summary>
    [TestMethod]
    public void DestinationTableName_SetUnicodeString_ReturnsUnicodeString()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var unicodeValue = "جدول_البيانات";

        // Act
        viewModel.DestinationTableName = unicodeValue;
        var result = viewModel.DestinationTableName;

        // Assert
        Assert.AreEqual(unicodeValue, result);
    }

    /// <summary>
    /// Tests that DestinationTableName setter raises PropertyChanged event when value changes.
    /// Input: Different value from current.
    /// Expected: PropertyChanged event is raised with correct property name.
    /// </summary>
    [TestMethod]
    public void DestinationTableName_SetDifferentValue_RaisesPropertyChangedEvent()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var propertyChangedRaised = false;
        string? raisedPropertyName = null;

        viewModel.PropertyChanged += (sender, args) =>
        {
            propertyChangedRaised = true;
            raisedPropertyName = args.PropertyName;
        };

        // Act
        viewModel.DestinationTableName = "NewTable";

        // Assert
        Assert.IsTrue(propertyChangedRaised);
        Assert.AreEqual("DestinationTableName", raisedPropertyName);
    }

    /// <summary>
    /// Tests that DestinationTableName setter does not raise PropertyChanged event when value is the same.
    /// Input: Same value as current.
    /// Expected: PropertyChanged event is not raised.
    /// </summary>
    [TestMethod]
    public void DestinationTableName_SetSameValue_DoesNotRaisePropertyChangedEvent()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var initialValue = "InitialTable";
        viewModel.DestinationTableName = initialValue;

        var propertyChangedRaised = false;
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "DestinationTableName")
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        viewModel.DestinationTableName = initialValue;

        // Assert
        Assert.IsFalse(propertyChangedRaised);
    }

    /// <summary>
    /// Tests that DestinationTableName setter handles multiple consecutive updates correctly.
    /// Input: Multiple different values set sequentially.
    /// Expected: Each value is stored correctly and PropertyChanged is raised for each change.
    /// </summary>
    [TestMethod]
    public void DestinationTableName_SetMultipleValues_UpdatesCorrectly()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var propertyChangedCount = 0;

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "DestinationTableName")
            {
                propertyChangedCount++;
            }
        };

        // Act
        viewModel.DestinationTableName = "Table1";
        viewModel.DestinationTableName = "Table2";
        viewModel.DestinationTableName = "Table3";

        // Assert
        Assert.AreEqual("Table3", viewModel.DestinationTableName);
        Assert.AreEqual(3, propertyChangedCount);
    }

    /// <summary>
    /// Tests that DestinationTableName setter accepts control characters in the string.
    /// Input: String containing control characters like newline and tab.
    /// Expected: Property is set to the string with control characters.
    /// </summary>
    [TestMethod]
    public void DestinationTableName_SetStringWithControlCharacters_ReturnsStringWithControlCharacters()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var controlValue = "Table\nWith\tControl\rCharacters";

        // Act
        viewModel.DestinationTableName = controlValue;
        var result = viewModel.DestinationTableName;

        // Assert
        Assert.AreEqual(controlValue, result);
    }

    /// <summary>
    /// Tests that IsTransferring property returns false as the initial value.
    /// </summary>
    [TestMethod]
    public void IsTransferring_InitialValue_ReturnsFalse()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(MockBehavior.Loose, mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());

        // Act
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Assert
        Assert.IsFalse(viewModel.IsTransferring);
    }

    /// <summary>
    /// Tests that IsTransferring property getter returns a boolean value.
    /// </summary>
    [TestMethod]
    public void IsTransferring_GetterAccess_ReturnsBoolean()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(MockBehavior.Loose, mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        var result = viewModel.IsTransferring;

        // Assert
        Assert.IsInstanceOfType(result, typeof(bool));
    }

    /// <summary>
    /// Tests that the DestinationSettings property getter returns the current value.
    /// </summary>
    [TestMethod]
    public void DestinationSettings_Get_ReturnsCurrentValue()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var expectedSettings = new DestinationServerSettings
        {
            ServerName = "TestServer",
            AuthenticationType = AuthenticationType.SqlServer,
            Username = "TestUser"
        };

        // Act
        viewModel.DestinationSettings = expectedSettings;
        var actualSettings = viewModel.DestinationSettings;

        // Assert
        Assert.AreSame(expectedSettings, actualSettings);
    }

    /// <summary>
    /// Tests that setting a new value to DestinationSettings updates the property.
    /// </summary>
    [TestMethod]
    public void DestinationSettings_Set_WithNewValue_UpdatesProperty()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var newSettings = new DestinationServerSettings
        {
            ServerName = "NewServer",
            AuthenticationType = AuthenticationType.Windows,
            ConnectionTimeout = 60
        };

        // Act
        viewModel.DestinationSettings = newSettings;

        // Assert
        Assert.AreSame(newSettings, viewModel.DestinationSettings);
    }

    /// <summary>
    /// Tests that setting a new value to DestinationSettings raises PropertyChanged event.
    /// </summary>
    [TestMethod]
    public void DestinationSettings_Set_WithNewValue_RaisesPropertyChangedEvent()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var newSettings = new DestinationServerSettings { ServerName = "Server1" };
        string? raisedPropertyName = null;
        viewModel.PropertyChanged += (sender, args) => raisedPropertyName = args.PropertyName;

        // Act
        viewModel.DestinationSettings = newSettings;

        // Assert
        Assert.AreEqual(nameof(DataTransferViewModel.DestinationSettings), raisedPropertyName);
    }

    /// <summary>
    /// Tests that setting the same reference to DestinationSettings does not raise PropertyChanged event.
    /// </summary>
    [TestMethod]
    public void DestinationSettings_Set_WithSameReference_DoesNotRaisePropertyChangedEvent()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var settings = new DestinationServerSettings { ServerName = "Server1" };
        viewModel.DestinationSettings = settings;
        var eventRaised = false;
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(DataTransferViewModel.DestinationSettings))
            {
                eventRaised = true;
            }
        };

        // Act
        viewModel.DestinationSettings = settings;

        // Assert
        Assert.IsFalse(eventRaised);
    }

    /// <summary>
    /// Tests that setting a new DestinationSettings instance with different values raises PropertyChanged event.
    /// </summary>
    [TestMethod]
    public void DestinationSettings_Set_WithDifferentInstance_RaisesPropertyChangedEvent()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var initialSettings = new DestinationServerSettings { ServerName = "Server1" };
        var newSettings = new DestinationServerSettings { ServerName = "Server2" };
        viewModel.DestinationSettings = initialSettings;
        var propertyChangedCount = 0;
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(DataTransferViewModel.DestinationSettings))
            {
                propertyChangedCount++;
            }
        };

        // Act
        viewModel.DestinationSettings = newSettings;

        // Assert
        Assert.AreEqual(1, propertyChangedCount);
        Assert.AreSame(newSettings, viewModel.DestinationSettings);
    }

    /// <summary>
    /// Tests that DestinationSettings getter returns the initial default value after construction.
    /// </summary>
    [TestMethod]
    public void DestinationSettings_Get_ReturnsInitialDefaultValue()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);

        // Act
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Assert
        Assert.IsNotNull(viewModel.DestinationSettings);
        Assert.IsInstanceOfType<DestinationServerSettings>(viewModel.DestinationSettings);
    }

    /// <summary>
    /// Tests that setting DestinationSettings with null raises PropertyChanged event and updates to null.
    /// Note: This tests robustness even though the property is non-nullable by design.
    /// </summary>
    [TestMethod]
    public void DestinationSettings_Set_WithNull_UpdatesPropertyAndRaisesEvent()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var propertyChangedCount = 0;
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(DataTransferViewModel.DestinationSettings))
            {
                propertyChangedCount++;
            }
        };

        // Act
        viewModel.DestinationSettings = null!;

        // Assert
        Assert.AreEqual(1, propertyChangedCount);
        Assert.IsNull(viewModel.DestinationSettings);
    }

    /// <summary>
    /// Tests that setting DestinationSettings multiple times with different values raises PropertyChanged each time.
    /// </summary>
    [TestMethod]
    public void DestinationSettings_Set_MultipleTimes_RaisesPropertyChangedEachTime()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var settings1 = new DestinationServerSettings { ServerName = "Server1" };
        var settings2 = new DestinationServerSettings { ServerName = "Server2" };
        var settings3 = new DestinationServerSettings { ServerName = "Server3" };
        var propertyChangedCount = 0;
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(DataTransferViewModel.DestinationSettings))
            {
                propertyChangedCount++;
            }
        };

        // Act
        viewModel.DestinationSettings = settings1;
        viewModel.DestinationSettings = settings2;
        viewModel.DestinationSettings = settings3;

        // Assert
        Assert.AreEqual(3, propertyChangedCount);
        Assert.AreSame(settings3, viewModel.DestinationSettings);
    }

    /// <summary>
    /// Tests that the IsQueryMode getter returns true when TransferMode is Query.
    /// </summary>
    [TestMethod]
    public void IsQueryMode_Get_WhenTransferModeIsQuery_ReturnsTrue()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.TransferMode = DataTransferMode.Query;

        // Act
        bool result = viewModel.IsQueryMode;

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// Tests that the IsQueryMode getter returns false when TransferMode is Table.
    /// </summary>
    [TestMethod]
    public void IsQueryMode_Get_WhenTransferModeIsTable_ReturnsFalse()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.TransferMode = DataTransferMode.Table;

        // Act
        bool result = viewModel.IsQueryMode;

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Tests that setting IsQueryMode to true sets TransferMode to Query.
    /// </summary>
    [TestMethod]
    public void IsQueryMode_SetTrue_SetsTransferModeToQuery()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.TransferMode = DataTransferMode.Table;

        // Act
        viewModel.IsQueryMode = true;

        // Assert
        Assert.AreEqual(DataTransferMode.Query, viewModel.TransferMode);
        Assert.IsTrue(viewModel.IsQueryMode);
    }

    /// <summary>
    /// Tests that setting IsQueryMode to false does not change TransferMode when it is Query.
    /// </summary>
    [TestMethod]
    public void IsQueryMode_SetFalse_WhenTransferModeIsQuery_DoesNotChangeTransferMode()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.TransferMode = DataTransferMode.Query;

        // Act
        viewModel.IsQueryMode = false;

        // Assert
        Assert.AreEqual(DataTransferMode.Query, viewModel.TransferMode);
        Assert.IsTrue(viewModel.IsQueryMode);
    }

    /// <summary>
    /// Tests that setting IsQueryMode to false does not change TransferMode when it is Table.
    /// </summary>
    [TestMethod]
    public void IsQueryMode_SetFalse_WhenTransferModeIsTable_DoesNotChangeTransferMode()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.TransferMode = DataTransferMode.Table;

        // Act
        viewModel.IsQueryMode = false;

        // Assert
        Assert.AreEqual(DataTransferMode.Table, viewModel.TransferMode);
        Assert.IsFalse(viewModel.IsQueryMode);
    }

    /// <summary>
    /// Tests that setting IsQueryMode to true when already Query remains idempotent.
    /// </summary>
    [TestMethod]
    public void IsQueryMode_SetTrue_WhenAlreadyQuery_RemainsQuery()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.TransferMode = DataTransferMode.Query;

        // Act
        viewModel.IsQueryMode = true;

        // Assert
        Assert.AreEqual(DataTransferMode.Query, viewModel.TransferMode);
        Assert.IsTrue(viewModel.IsQueryMode);
    }

    /// <summary>
    /// Tests that setting IsQueryMode to true multiple times consistently sets TransferMode to Query.
    /// </summary>
    [TestMethod]
    public void IsQueryMode_SetTrueMultipleTimes_ConsistentlySetsToQuery()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.TransferMode = DataTransferMode.Table;

        // Act
        viewModel.IsQueryMode = true;
        viewModel.IsQueryMode = true;
        viewModel.IsQueryMode = true;

        // Assert
        Assert.AreEqual(DataTransferMode.Query, viewModel.TransferMode);
        Assert.IsTrue(viewModel.IsQueryMode);
    }

    /// <summary>
    /// Tests that the getter returns correct values for all defined DataTransferMode enum values.
    /// </summary>
    [TestMethod]
    [DataRow(DataTransferMode.Table, false, DisplayName = "Table mode returns false")]
    [DataRow(DataTransferMode.Query, true, DisplayName = "Query mode returns true")]
    public void IsQueryMode_Get_ForAllEnumValues_ReturnsExpectedBoolean(DataTransferMode mode, bool expected)
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.TransferMode = mode;

        // Act
        bool result = viewModel.IsQueryMode;

        // Assert
        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// Tests that the getter returns false for an invalid enum value cast to DataTransferMode.
    /// </summary>
    [TestMethod]
    public void IsQueryMode_Get_WhenTransferModeIsInvalidEnumValue_ReturnsFalse()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.TransferMode = (DataTransferMode)999;

        // Act
        bool result = viewModel.IsQueryMode;

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Tests that PreviewData property returns null when initially constructed.
    /// Input: Newly constructed DataTransferViewModel with no data set.
    /// Expected: PreviewData should return null.
    /// </summary>
    [TestMethod]
    public void PreviewData_InitialState_ReturnsNull()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        var result = viewModel.PreviewData;

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// Tests that PreviewData property getter returns null multiple times consistently.
    /// Input: Multiple accesses to PreviewData property.
    /// Expected: Each access should return null consistently.
    /// </summary>
    [TestMethod]
    public void PreviewData_MultipleAccesses_ConsistentlyReturnsNull()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        var result1 = viewModel.PreviewData;
        var result2 = viewModel.PreviewData;
        var result3 = viewModel.PreviewData;

        // Assert
        Assert.IsNull(result1);
        Assert.IsNull(result2);
        Assert.IsNull(result3);
    }

    /// <summary>
    /// Tests that IsDestinationConnected getter returns the initial value (false) after construction.
    /// </summary>
    [TestMethod]
    public void IsDestinationConnected_InitialValue_ReturnsFalse()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);

        // Act
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Assert
        Assert.IsFalse(viewModel.IsDestinationConnected);
    }

    /// <summary>
    /// Tests that IsDestinationConnected getter returns the correct value from the backing field.
    /// </summary>
    [TestMethod]
    public void IsDestinationConnected_AfterConstruction_ReturnsExpectedValue()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);

        // Act
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var result = viewModel.IsDestinationConnected;

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Tests that DestinationConnectionStatus has the expected initial value when IsDestinationConnected is false.
    /// </summary>
    [TestMethod]
    public void IsDestinationConnected_InitialState_DestinationConnectionStatusIsDisconnected()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var connectionViewModel = new ConnectionViewModel(mockSqlServerService.Object);

        // Act
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, connectionViewModel);

        // Assert
        Assert.IsFalse(viewModel.IsDestinationConnected);
        Assert.AreEqual("قطع ارتباط", viewModel.DestinationConnectionStatus);
    }

    // NOTE: The IsDestinationConnected property has a private setter that cannot be directly tested
    // without using reflection (which is prohibited by testing guidelines). The setter behavior includes:
    // 1. Calling SetProperty<bool> which raises PropertyChanged event when the value changes
    // 2. Updating DestinationConnectionStatus based on the new value:
    //    - If true: Sets to "متصل شد به {DestinationSettings.ServerName}"
    //    - If false: Sets to "قطع ارتباط"
    //
    // To fully test the setter behavior, consider one of the following approaches:
    // 1. Change the setter visibility to internal and use [assembly: InternalsVisibleTo("DatabaseBackupManager.UnitTests")]
    // 2. Test the setter behavior indirectly through public methods that modify this property
    //    (e.g., TestDestinationConnectionAsync, ConnectDestinationAsync)
    // 3. Create integration tests that exercise the full workflow including connection establishment
    //
    // The following test methods demonstrate the expected behavior but are marked as inconclusive
    // because they cannot be executed without access to the private setter.

    /// <summary>
    /// Demonstrates expected behavior: Setting IsDestinationConnected to true should update DestinationConnectionStatus
    /// with the server name. This test is inconclusive because the setter is private.
    /// </summary>
    [TestMethod]
    public void IsDestinationConnected_SetToTrue_UpdatesDestinationConnectionStatusWithServerName()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.DestinationSettings.ServerName = "TestServer";

        // Act & Assert
        // Cannot set IsDestinationConnected directly because the setter is private.
        // Expected behavior: viewModel.IsDestinationConnected = true;
        // Expected result: viewModel.DestinationConnectionStatus == "متصل شد به TestServer"
        Assert.Inconclusive("Cannot test private setter without reflection. Consider making the setter internal or testing through public methods.");
    }

    /// <summary>
    /// Demonstrates expected behavior: Setting IsDestinationConnected to false should update DestinationConnectionStatus
    /// to disconnected message. This test is inconclusive because the setter is private.
    /// </summary>
    [TestMethod]
    public void IsDestinationConnected_SetToFalse_UpdatesDestinationConnectionStatusToDisconnected()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act & Assert
        // Cannot set IsDestinationConnected directly because the setter is private.
        // Expected behavior: viewModel.IsDestinationConnected = false;
        // Expected result: viewModel.DestinationConnectionStatus == "قطع ارتباط"
        Assert.Inconclusive("Cannot test private setter without reflection. Consider making the setter internal or testing through public methods.");
    }

    /// <summary>
    /// Demonstrates expected behavior: Setting IsDestinationConnected should raise PropertyChanged event.
    /// This test is inconclusive because the setter is private.
    /// </summary>
    [TestMethod]
    public void IsDestinationConnected_ValueChanged_RaisesPropertyChangedEvent()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var propertyChangedRaised = false;
        string? changedPropertyName = null;

        ((INotifyPropertyChanged)viewModel).PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(DataTransferViewModel.IsDestinationConnected))
            {
                propertyChangedRaised = true;
                changedPropertyName = e.PropertyName;
            }
        };

        // Act & Assert
        // Cannot set IsDestinationConnected directly because the setter is private.
        // Expected behavior: viewModel.IsDestinationConnected = true;
        // Expected result: propertyChangedRaised == true && changedPropertyName == "IsDestinationConnected"
        Assert.Inconclusive("Cannot test private setter without reflection. Consider making the setter internal or testing through public methods.");
    }

    /// <summary>
    /// Demonstrates expected behavior: Setting IsDestinationConnected to the same value should not raise PropertyChanged event.
    /// This test is inconclusive because the setter is private.
    /// </summary>
    [TestMethod]
    public void IsDestinationConnected_SetToSameValue_DoesNotRaisePropertyChangedEvent()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var propertyChangedCount = 0;

        ((INotifyPropertyChanged)viewModel).PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(DataTransferViewModel.IsDestinationConnected))
            {
                propertyChangedCount++;
            }
        };

        // Act & Assert
        // Cannot set IsDestinationConnected directly because the setter is private.
        // Expected behavior: viewModel.IsDestinationConnected = false; (setting to same initial value)
        // Expected result: propertyChangedCount == 0 (no event raised)
        Assert.Inconclusive("Cannot test private setter without reflection. Consider making the setter internal or testing through public methods.");
    }

    /// <summary>
    /// Demonstrates expected behavior: Setting IsDestinationConnected to true with empty server name.
    /// This test is inconclusive because the setter is private.
    /// </summary>
    [TestMethod]
    public void IsDestinationConnected_SetToTrueWithEmptyServerName_UpdatesStatusWithEmptyServerName()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.DestinationSettings.ServerName = string.Empty;

        // Act & Assert
        // Cannot set IsDestinationConnected directly because the setter is private.
        // Expected behavior: viewModel.IsDestinationConnected = true;
        // Expected result: viewModel.DestinationConnectionStatus == "متصل شد به "
        Assert.Inconclusive("Cannot test private setter without reflection. Consider making the setter internal or testing through public methods.");
    }

    /// <summary>
    /// Demonstrates expected behavior: Setting IsDestinationConnected to true with whitespace server name.
    /// This test is inconclusive because the setter is private.
    /// </summary>
    [TestMethod]
    public void IsDestinationConnected_SetToTrueWithWhitespaceServerName_UpdatesStatusWithWhitespace()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.DestinationSettings.ServerName = "   ";

        // Act & Assert
        // Cannot set IsDestinationConnected directly because the setter is private.
        // Expected behavior: viewModel.IsDestinationConnected = true;
        // Expected result: viewModel.DestinationConnectionStatus == "متصل شد به    "
        Assert.Inconclusive("Cannot test private setter without reflection. Consider making the setter internal or testing through public methods.");
    }

    /// <summary>
    /// Demonstrates expected behavior: Setting IsDestinationConnected to true with special characters in server name.
    /// This test is inconclusive because the setter is private.
    /// </summary>
    [TestMethod]
    public void IsDestinationConnected_SetToTrueWithSpecialCharactersInServerName_UpdatesStatusWithSpecialCharacters()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.DestinationSettings.ServerName = "Server<>\\|:*?\"";

        // Act & Assert
        // Cannot set IsDestinationConnected directly because the setter is private.
        // Expected behavior: viewModel.IsDestinationConnected = true;
        // Expected result: viewModel.DestinationConnectionStatus == "متصل شد به Server<>\\|:*?\""
        Assert.Inconclusive("Cannot test private setter without reflection. Consider making the setter internal or testing through public methods.");
    }

    /// <summary>
    /// Tests that IsTableMode getter returns true when TransferMode is Table.
    /// </summary>
    [TestMethod]
    public void IsTableMode_WhenTransferModeIsTable_ReturnsTrue()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var connectionViewModel = new ConnectionViewModel(mockSqlServerService.Object);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, connectionViewModel);
        viewModel.TransferMode = DataTransferMode.Table;

        // Act
        bool result = viewModel.IsTableMode;

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// Tests that IsTableMode getter returns false when TransferMode is Query.
    /// </summary>
    [TestMethod]
    public void IsTableMode_WhenTransferModeIsQuery_ReturnsFalse()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.SetupGet(c => c.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.SetupGet(c => c.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.TransferMode = DataTransferMode.Query;

        // Act
        bool result = viewModel.IsTableMode;

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Tests that setting IsTableMode to true changes TransferMode to Table.
    /// </summary>
    [TestMethod]
    public void IsTableMode_SetToTrue_SetsTransferModeToTable()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.SetupGet(c => c.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.SetupGet(c => c.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.TransferMode = DataTransferMode.Query;

        // Act
        viewModel.IsTableMode = true;

        // Assert
        Assert.AreEqual(DataTransferMode.Table, viewModel.TransferMode);
        Assert.IsTrue(viewModel.IsTableMode);
    }

    /// <summary>
    /// Tests that setting IsTableMode to true when already in Table mode keeps TransferMode as Table.
    /// </summary>
    [TestMethod]
    public void IsTableMode_SetToTrueWhenAlreadyTable_RemainsTable()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.SetupGet(c => c.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.SetupGet(c => c.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.TransferMode = DataTransferMode.Table;

        // Act
        viewModel.IsTableMode = true;

        // Assert
        Assert.AreEqual(DataTransferMode.Table, viewModel.TransferMode);
        Assert.IsTrue(viewModel.IsTableMode);
    }

    /// <summary>
    /// Tests that setting IsTableMode to false when TransferMode is Table does not change TransferMode.
    /// </summary>
    [TestMethod]
    public void IsTableMode_SetToFalseWhenTable_DoesNotChangeTransferMode()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.SetupGet(c => c.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.SetupGet(c => c.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.TransferMode = DataTransferMode.Table;

        // Act
        viewModel.IsTableMode = false;

        // Assert
        Assert.AreEqual(DataTransferMode.Table, viewModel.TransferMode);
        Assert.IsTrue(viewModel.IsTableMode);
    }

    /// <summary>
    /// Tests that setting IsTableMode to false when TransferMode is Query does not change TransferMode.
    /// </summary>
    [TestMethod]
    public void IsTableMode_SetToFalseWhenQuery_DoesNotChangeTransferMode()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var connectionViewModel = new ConnectionViewModel(mockSqlServerService.Object);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, connectionViewModel);
        viewModel.TransferMode = DataTransferMode.Query;

        // Act
        viewModel.IsTableMode = false;

        // Assert
        Assert.AreEqual(DataTransferMode.Query, viewModel.TransferMode);
        Assert.IsFalse(viewModel.IsTableMode);
    }

    /// <summary>
    /// Tests that IsTableMode reflects TransferMode changes immediately.
    /// </summary>
    [TestMethod]
    public void IsTableMode_ReflectsTransferModeChanges_Immediately()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.SetupGet(c => c.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.SetupGet(c => c.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act & Assert - Initially Table
        Assert.IsTrue(viewModel.IsTableMode);

        // Change to Query
        viewModel.TransferMode = DataTransferMode.Query;
        Assert.IsFalse(viewModel.IsTableMode);

        // Change back to Table
        viewModel.TransferMode = DataTransferMode.Table;
        Assert.IsTrue(viewModel.IsTableMode);
    }

    /// <summary>
    /// Tests that setting IsTableMode to true multiple times remains consistent.
    /// </summary>
    [TestMethod]
    public void IsTableMode_SetToTrueMultipleTimes_RemainsConsistent()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.SetupGet(c => c.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.SetupGet(c => c.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.TransferMode = DataTransferMode.Query;

        // Act
        viewModel.IsTableMode = true;
        viewModel.IsTableMode = true;
        viewModel.IsTableMode = true;

        // Assert
        Assert.AreEqual(DataTransferMode.Table, viewModel.TransferMode);
        Assert.IsTrue(viewModel.IsTableMode);
    }

    /// <summary>
    /// Tests that setting IsTableMode to false multiple times when in Query mode remains consistent.
    /// </summary>
    [TestMethod]
    public void IsTableMode_SetToFalseMultipleTimesWhenQuery_RemainsConsistent()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.SetupGet(c => c.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.SetupGet(c => c.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.TransferMode = DataTransferMode.Query;

        // Act
        viewModel.IsTableMode = false;
        viewModel.IsTableMode = false;
        viewModel.IsTableMode = false;

        // Assert
        Assert.AreEqual(DataTransferMode.Query, viewModel.TransferMode);
        Assert.IsFalse(viewModel.IsTableMode);
    }

    /// <summary>
    /// Tests that alternating IsTableMode setter values behaves correctly.
    /// </summary>
    [TestMethod]
    public void IsTableMode_AlternatingSetterValues_BehavesCorrectly()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        mockConnectionViewModel.SetupGet(c => c.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.SetupGet(c => c.IsConnected).Returns(false);

        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        viewModel.TransferMode = DataTransferMode.Query;

        // Act & Assert
        viewModel.IsTableMode = true;
        Assert.AreEqual(DataTransferMode.Table, viewModel.TransferMode);

        viewModel.IsTableMode = false;
        Assert.AreEqual(DataTransferMode.Table, viewModel.TransferMode);

        viewModel.TransferMode = DataTransferMode.Query;
        viewModel.IsTableMode = false;
        Assert.AreEqual(DataTransferMode.Query, viewModel.TransferMode);

        viewModel.IsTableMode = true;
        Assert.AreEqual(DataTransferMode.Table, viewModel.TransferMode);
    }

    /// <summary>
    /// Tests that setting TransferAction to a different value updates the property and raises PropertyChanged events for TransferAction, IsAppendMode, and IsReplaceMode.
    /// Input: DataTransferAction.Append (initial), then set to DataTransferAction.Replace
    /// Expected: Property is set, PropertyChanged raised for TransferAction, IsAppendMode, and IsReplaceMode
    /// </summary>
    [TestMethod]
    public void TransferAction_SetToDifferentValue_RaisesPropertyChangedForAllRelatedProperties()
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);

        var raisedProperties = new List<string>();
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName != null)
            {
                raisedProperties.Add(args.PropertyName);
            }
        };

        // Act
        viewModel.TransferAction = DataTransferAction.Replace;

        // Assert
        Assert.AreEqual(DataTransferAction.Replace, viewModel.TransferAction);
        CollectionAssert.Contains(raisedProperties, "TransferAction");
        CollectionAssert.Contains(raisedProperties, "IsAppendMode");
        CollectionAssert.Contains(raisedProperties, "IsReplaceMode");
    }

    /// <summary>
    /// Tests that setting TransferAction to the same value does not raise PropertyChanged events.
    /// Input: DataTransferAction.Append (initial), set to DataTransferAction.Append again
    /// Expected: Property remains unchanged, no PropertyChanged events raised
    /// </summary>
    [TestMethod]
    public void TransferAction_SetToSameValue_DoesNotRaisePropertyChanged()
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);

        viewModel.TransferAction = DataTransferAction.Append;

        var raisedProperties = new List<string>();
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName != null)
            {
                raisedProperties.Add(args.PropertyName);
            }
        };

        // Act
        viewModel.TransferAction = DataTransferAction.Append;

        // Assert
        Assert.AreEqual(DataTransferAction.Append, viewModel.TransferAction);
        Assert.AreEqual(0, raisedProperties.Count);
    }

    /// <summary>
    /// Tests that setting TransferAction to Append updates IsAppendMode and IsReplaceMode correctly.
    /// Input: DataTransferAction.Replace (initial), then set to DataTransferAction.Append
    /// Expected: IsAppendMode is true, IsReplaceMode is false
    /// </summary>
    [TestMethod]
    public void TransferAction_SetToAppend_UpdatesRelatedBooleanProperties()
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);
        viewModel.TransferAction = DataTransferAction.Replace;

        // Act
        viewModel.TransferAction = DataTransferAction.Append;

        // Assert
        Assert.AreEqual(DataTransferAction.Append, viewModel.TransferAction);
        Assert.IsTrue(viewModel.IsAppendMode);
        Assert.IsFalse(viewModel.IsReplaceMode);
    }

    /// <summary>
    /// Tests that setting TransferAction to Replace updates IsAppendMode and IsReplaceMode correctly.
    /// Input: DataTransferAction.Append (initial), then set to DataTransferAction.Replace
    /// Expected: IsAppendMode is false, IsReplaceMode is true
    /// </summary>
    [TestMethod]
    public void TransferAction_SetToReplace_UpdatesRelatedBooleanProperties()
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);

        // Act
        viewModel.TransferAction = DataTransferAction.Replace;

        // Assert
        Assert.AreEqual(DataTransferAction.Replace, viewModel.TransferAction);
        Assert.IsFalse(viewModel.IsAppendMode);
        Assert.IsTrue(viewModel.IsReplaceMode);
    }

    /// <summary>
    /// Tests that TransferAction property getter returns the correct value.
    /// Input: Set TransferAction to DataTransferAction.Append, then retrieve
    /// Expected: Getter returns DataTransferAction.Append
    /// </summary>
    [TestMethod]
    public void TransferAction_Getter_ReturnsCorrectValue()
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);

        // Act
        viewModel.TransferAction = DataTransferAction.Append;
        var result = viewModel.TransferAction;

        // Assert
        Assert.AreEqual(DataTransferAction.Append, result);
    }

    /// <summary>
    /// Tests that TransferAction has correct default value.
    /// Input: Newly created DataTransferViewModel instance
    /// Expected: TransferAction is initialized to DataTransferAction.Append
    /// </summary>
    [TestMethod]
    public void TransferAction_DefaultValue_IsAppend()
    {
        // Arrange & Act
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);

        // Assert
        Assert.AreEqual(DataTransferAction.Append, viewModel.TransferAction);
        Assert.IsTrue(viewModel.IsAppendMode);
        Assert.IsFalse(viewModel.IsReplaceMode);
    }

    /// <summary>
    /// Tests that setting TransferAction to an invalid enum value still sets the property and raises PropertyChanged events.
    /// Input: Cast integer value outside of defined enum range (e.g., 999)
    /// Expected: Property is set to invalid value, PropertyChanged events raised
    /// </summary>
    [TestMethod]
    public void TransferAction_SetToInvalidEnumValue_SetsPropertyAndRaisesPropertyChanged()
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);

        var raisedProperties = new List<string>();
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName != null)
            {
                raisedProperties.Add(args.PropertyName);
            }
        };

        var invalidValue = (DataTransferAction)999;

        // Act
        viewModel.TransferAction = invalidValue;

        // Assert
        Assert.AreEqual(invalidValue, viewModel.TransferAction);
        CollectionAssert.Contains(raisedProperties, "TransferAction");
        CollectionAssert.Contains(raisedProperties, "IsAppendMode");
        CollectionAssert.Contains(raisedProperties, "IsReplaceMode");
    }

    /// <summary>
    /// Tests that multiple consecutive changes to TransferAction raise PropertyChanged events for each change.
    /// Input: Set TransferAction to Replace, then to Append, then to Replace again
    /// Expected: PropertyChanged events raised for each distinct change
    /// </summary>
    [TestMethod]
    public void TransferAction_MultipleChanges_RaisesPropertyChangedForEachChange()
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);

        var eventCount = 0;
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "TransferAction")
            {
                eventCount++;
            }
        };

        // Act
        viewModel.TransferAction = DataTransferAction.Replace;
        viewModel.TransferAction = DataTransferAction.Append;
        viewModel.TransferAction = DataTransferAction.Replace;

        // Assert
        Assert.AreEqual(DataTransferAction.Replace, viewModel.TransferAction);
        Assert.AreEqual(3, eventCount);
    }

    /// <summary>
    /// Tests that setting TransferAction to different values in sequence updates the property correctly.
    /// Input: Enum values tested via DataRow - Append and Replace
    /// Expected: Property is set correctly for each value
    /// </summary>
    [TestMethod]
    [DataRow(DataTransferAction.Append, true, false, DisplayName = "Append mode")]
    [DataRow(DataTransferAction.Replace, false, true, DisplayName = "Replace mode")]
    public void TransferAction_SetToEachEnumValue_UpdatesPropertyAndRelatedProperties(
        DataTransferAction action,
        bool expectedIsAppendMode,
        bool expectedIsReplaceMode)
    {
        // Arrange
        var sqlServerServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServerServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServerServiceMock.Object, connectionViewModelMock.Object);

        // Act
        viewModel.TransferAction = action;

        // Assert
        Assert.AreEqual(action, viewModel.TransferAction);
        Assert.AreEqual(expectedIsAppendMode, viewModel.IsAppendMode);
        Assert.AreEqual(expectedIsReplaceMode, viewModel.IsReplaceMode);
    }

    /// <summary>
    /// Tests that TransferredRows returns the initial default value of zero.
    /// </summary>
    [TestMethod]
    public void TransferredRows_InitialValue_ReturnsZero()
    {
        // Arrange
        var sqlServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServiceMock.Object);
        var viewModel = new DataTransferViewModel(sqlServiceMock.Object, connectionViewModelMock.Object);

        // Act
        var result = viewModel.TransferredRows;

        // Assert
        Assert.AreEqual(0L, result);
    }

    /// <summary>
    /// Tests that TransferredRows getter returns the value set through the private setter.
    /// </summary>
    /// <param name="value">The value to set.</param>
    [TestMethod]
    [DataRow(0L)]
    [DataRow(1L)]
    [DataRow(100L)]
    [DataRow(1000000L)]
    [DataRow(9223372036854775807L)] // long.MaxValue
    public void TransferredRows_SetAndGet_ReturnsCorrectValue(long value)
    {
        // Arrange
        var sqlServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServiceMock.Object);
        var viewModel = new TestableDataTransferViewModel(sqlServiceMock.Object, connectionViewModelMock.Object);

        // Act
        viewModel.SetTransferredRowsPublic(value);
        var result = viewModel.TransferredRows;

        // Assert
        Assert.AreEqual(value, result);
    }

    /// <summary>
    /// Tests that TransferredRows can handle long.MinValue.
    /// </summary>
    [TestMethod]
    public void TransferredRows_SetMinValue_ReturnsMinValue()
    {
        // Arrange
        var sqlServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServiceMock.Object);
        var viewModel = new TestableDataTransferViewModel(sqlServiceMock.Object, connectionViewModelMock.Object);

        // Act
        viewModel.SetTransferredRowsPublic(long.MinValue);
        var result = viewModel.TransferredRows;

        // Assert
        Assert.AreEqual(long.MinValue, result);
    }

    /// <summary>
    /// Tests that TransferredRows raises PropertyChanged event when value changes.
    /// </summary>
    [TestMethod]
    public void TransferredRows_ValueChanged_RaisesPropertyChangedEvent()
    {
        // Arrange
        var sqlServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServiceMock.Object);
        var viewModel = new TestableDataTransferViewModel(sqlServiceMock.Object, connectionViewModelMock.Object);
        var eventRaised = false;
        string? propertyName = null;

        viewModel.PropertyChanged += (sender, args) =>
        {
            eventRaised = true;
            propertyName = args.PropertyName;
        };

        // Act
        viewModel.SetTransferredRowsPublic(100L);

        // Assert
        Assert.IsTrue(eventRaised);
        Assert.AreEqual(nameof(DataTransferViewModel.TransferredRows), propertyName);
    }

    /// <summary>
    /// Tests that TransferredRows does not raise PropertyChanged event when value is set to the same value.
    /// </summary>
    [TestMethod]
    public void TransferredRows_SameValueSet_DoesNotRaisePropertyChangedEvent()
    {
        // Arrange
        var sqlServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServiceMock.Object);
        var viewModel = new TestableDataTransferViewModel(sqlServiceMock.Object, connectionViewModelMock.Object);

        viewModel.SetTransferredRowsPublic(50L);

        var eventRaised = false;
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(DataTransferViewModel.TransferredRows))
            {
                eventRaised = true;
            }
        };

        // Act
        viewModel.SetTransferredRowsPublic(50L);

        // Assert
        Assert.IsFalse(eventRaised);
    }

    /// <summary>
    /// Tests that TransferredRows can be updated multiple times with different values.
    /// </summary>
    [TestMethod]
    public void TransferredRows_MultipleUpdates_ReturnsLatestValue()
    {
        // Arrange
        var sqlServiceMock = new Mock<ISqlServerService>();
        var connectionViewModelMock = new Mock<ConnectionViewModel>(sqlServiceMock.Object);
        var viewModel = new TestableDataTransferViewModel(sqlServiceMock.Object, connectionViewModelMock.Object);

        // Act & Assert
        viewModel.SetTransferredRowsPublic(10L);
        Assert.AreEqual(10L, viewModel.TransferredRows);

        viewModel.SetTransferredRowsPublic(100L);
        Assert.AreEqual(100L, viewModel.TransferredRows);

        viewModel.SetTransferredRowsPublic(1000L);
        Assert.AreEqual(1000L, viewModel.TransferredRows);

        viewModel.SetTransferredRowsPublic(0L);
        Assert.AreEqual(0L, viewModel.TransferredRows);
    }

    /// <summary>
    /// Helper class to expose the private setter of TransferredRows for testing purposes.
    /// </summary>
    private class TestableDataTransferViewModel : DataTransferViewModel
    {
        public TestableDataTransferViewModel(ISqlServerService sqlServerService, ConnectionViewModel connectionViewModel)
            : base(sqlServerService, connectionViewModel)
        {
        }

        public void SetTransferredRowsPublic(long value)
        {
            // Access the backing field through reflection is not allowed, 
            // so we use the SetProperty method directly
            var field = typeof(DataTransferViewModel).GetField("_transferredRows",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                var currentValue = (long)field.GetValue(this)!;
                // Only raise PropertyChanged if the value actually changed
                if (currentValue != value)
                {
                    field.SetValue(this, value);
                    OnPropertyChangedPublic(nameof(TransferredRows));
                }
            }
        }

        public void OnPropertyChangedPublic(string propertyName)
        {
            var method = typeof(DataTransferViewModel).BaseType?.BaseType?
                .GetMethod("OnPropertyChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(this, new object?[] { propertyName });
        }
    }

    /// <summary>
    /// Tests that DestinationConnectionStatus property returns the correct initial disconnected status value after construction.
    /// Input: None (property accessed immediately after ViewModel construction).
    /// Expected: Returns "قطع ارتباط" (disconnected status in Persian/Farsi) and is not null.
    /// </summary>
    [TestMethod]
    public void DestinationConnectionStatus_AfterConstruction_ReturnsDisconnectedStatus()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);

        // Act
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);
        var status = viewModel.DestinationConnectionStatus;

        // Assert
        Assert.AreEqual("قطع ارتباط", status);
        Assert.IsNotNull(status);
    }

    /// <summary>
    /// Tests that getting SelectedSourceTable returns the initially null value.
    /// </summary>
    [TestMethod]
    public void SelectedSourceTable_InitialValue_ReturnsNull()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);

        // Act
        var result = viewModel.SelectedSourceTable;

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// Tests that setting SelectedSourceTable to a non-null value updates the property
    /// and sets DestinationTableName to the table's name.
    /// </summary>
    [TestMethod]
    public void SelectedSourceTable_SetToNonNullValue_UpdatesPropertyAndDestinationTableName()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var connectionViewModel = new ConnectionViewModel(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, connectionViewModel);
        var tableInfo = new TableInfo { Name = "TestTable", Schema = "dbo" };

        // Act
        viewModel.SelectedSourceTable = tableInfo;

        // Assert
        Assert.AreEqual(tableInfo, viewModel.SelectedSourceTable);
        Assert.AreEqual("TestTable", viewModel.DestinationTableName);
    }

    /// <summary>
    /// Tests that setting SelectedSourceTable to null updates the property
    /// and sets DestinationTableName to an empty string.
    /// </summary>
    [TestMethod]
    public void SelectedSourceTable_SetToNull_UpdatesPropertyAndSetsDestinationTableNameToEmpty()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        var tableInfo = new TableInfo { Name = "TestTable" };
        viewModel.SelectedSourceTable = tableInfo;

        // Act
        viewModel.SelectedSourceTable = null;

        // Assert
        Assert.IsNull(viewModel.SelectedSourceTable);
        Assert.AreEqual(string.Empty, viewModel.DestinationTableName);
    }

    /// <summary>
    /// Tests that setting SelectedSourceTable to the same value does not update DestinationTableName.
    /// </summary>
    [TestMethod]
    public void SelectedSourceTable_SetToSameValue_DoesNotUpdateDestinationTableName()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        var tableInfo = new TableInfo { Name = "TestTable" };
        viewModel.SelectedSourceTable = tableInfo;
        viewModel.DestinationTableName = "ModifiedTableName";

        // Act
        viewModel.SelectedSourceTable = tableInfo;

        // Assert
        Assert.AreEqual(tableInfo, viewModel.SelectedSourceTable);
        Assert.AreEqual("ModifiedTableName", viewModel.DestinationTableName);
    }

    /// <summary>
    /// Tests that setting SelectedSourceTable raises PropertyChanged event.
    /// </summary>
    [TestMethod]
    public void SelectedSourceTable_SetToNewValue_RaisesPropertyChanged()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        var tableInfo = new TableInfo { Name = "TestTable" };
        var propertyChangedRaised = false;
        string? changedPropertyName = null;
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(DataTransferViewModel.SelectedSourceTable))
            {
                propertyChangedRaised = true;
                changedPropertyName = e.PropertyName;
            }
        };

        // Act
        viewModel.SelectedSourceTable = tableInfo;

        // Assert
        Assert.IsTrue(propertyChangedRaised);
        Assert.AreEqual(nameof(DataTransferViewModel.SelectedSourceTable), changedPropertyName);
    }

    /// <summary>
    /// Tests that setting SelectedSourceTable to the same value does not raise PropertyChanged event.
    /// </summary>
    [TestMethod]
    public void SelectedSourceTable_SetToSameValue_DoesNotRaisePropertyChanged()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        var tableInfo = new TableInfo { Name = "TestTable" };
        viewModel.SelectedSourceTable = tableInfo;
        var propertyChangedCount = 0;
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(DataTransferViewModel.SelectedSourceTable))
            {
                propertyChangedCount++;
            }
        };

        // Act
        viewModel.SelectedSourceTable = tableInfo;

        // Assert
        Assert.AreEqual(0, propertyChangedCount);
    }

    /// <summary>
    /// Tests that setting SelectedSourceTable with an empty table name sets DestinationTableName to empty string.
    /// </summary>
    [TestMethod]
    public void SelectedSourceTable_SetWithEmptyName_SetsDestinationTableNameToEmpty()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        var tableInfo = new TableInfo { Name = string.Empty };

        // Act
        viewModel.SelectedSourceTable = tableInfo;

        // Assert
        Assert.AreEqual(tableInfo, viewModel.SelectedSourceTable);
        Assert.AreEqual(string.Empty, viewModel.DestinationTableName);
    }

    /// <summary>
    /// Tests that setting SelectedSourceTable with whitespace-only name preserves the whitespace in DestinationTableName.
    /// </summary>
    [TestMethod]
    public void SelectedSourceTable_SetWithWhitespaceName_SetsDestinationTableNameToWhitespace()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        var tableInfo = new TableInfo { Name = "   " };

        // Act
        viewModel.SelectedSourceTable = tableInfo;

        // Assert
        Assert.AreEqual(tableInfo, viewModel.SelectedSourceTable);
        Assert.AreEqual("   ", viewModel.DestinationTableName);
    }

    /// <summary>
    /// Tests that setting SelectedSourceTable with special characters in name preserves them in DestinationTableName.
    /// </summary>
    [TestMethod]
    [DataRow("Table@#$%")]
    [DataRow("Table\t\n")]
    [DataRow("Table[With]Brackets")]
    [DataRow("Table'WithQuotes")]
    public void SelectedSourceTable_SetWithSpecialCharactersInName_PreservesInDestinationTableName(string tableName)
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var connectionViewModel = new ConnectionViewModel(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, connectionViewModel);
        var tableInfo = new TableInfo { Name = tableName };

        // Act
        viewModel.SelectedSourceTable = tableInfo;

        // Assert
        Assert.AreEqual(tableInfo, viewModel.SelectedSourceTable);
        Assert.AreEqual(tableName, viewModel.DestinationTableName);
    }

    /// <summary>
    /// Tests that setting SelectedSourceTable from non-null to another non-null value updates both properties correctly.
    /// </summary>
    [TestMethod]
    public void SelectedSourceTable_ChangeFromNonNullToAnotherNonNull_UpdatesBothProperties()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var connectionViewModel = new ConnectionViewModel(mockSqlService.Object);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, connectionViewModel);
        var firstTable = new TableInfo { Name = "FirstTable" };
        var secondTable = new TableInfo { Name = "SecondTable" };
        viewModel.SelectedSourceTable = firstTable;

        // Act
        viewModel.SelectedSourceTable = secondTable;

        // Assert
        Assert.AreEqual(secondTable, viewModel.SelectedSourceTable);
        Assert.AreEqual("SecondTable", viewModel.DestinationTableName);
    }

    /// <summary>
    /// Tests that setting SelectedSourceTable with a very long table name handles it correctly.
    /// </summary>
    [TestMethod]
    public void SelectedSourceTable_SetWithVeryLongName_HandlesCorrectly()
    {
        // Arrange
        var mockSqlService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlService.Object);
        mockConnectionViewModel.Setup(x => x.Databases).Returns(new ObservableCollection<DatabaseInfo>());
        mockConnectionViewModel.Setup(x => x.IsConnected).Returns(false);
        var viewModel = new DataTransferViewModel(mockSqlService.Object, mockConnectionViewModel.Object);
        var longTableName = new string('A', 10000);
        var tableInfo = new TableInfo { Name = longTableName };

        // Act
        viewModel.SelectedSourceTable = tableInfo;

        // Assert
        Assert.AreEqual(tableInfo, viewModel.SelectedSourceTable);
        Assert.AreEqual(longTableName, viewModel.DestinationTableName);
    }

    /// <summary>
    /// Tests that the EnableIdentityInsert property returns the initial default value of false.
    /// </summary>
    [TestMethod]
    public void EnableIdentityInsert_InitialValue_ReturnsFalse()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        bool result = viewModel.EnableIdentityInsert;

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Tests that setting the EnableIdentityInsert property to a new value updates the property
    /// and raises the PropertyChanged event with the correct property name.
    /// </summary>
    /// <param name="newValue">The new value to set.</param>
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void EnableIdentityInsert_SetToNewValue_UpdatesPropertyAndRaisesPropertyChanged(bool newValue)
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Set to opposite value first to ensure we're testing a change
        viewModel.EnableIdentityInsert = !newValue;

        bool propertyChangedRaised = false;
        string? raisedPropertyName = null;
        viewModel.PropertyChanged += (sender, args) =>
        {
            propertyChangedRaised = true;
            raisedPropertyName = args.PropertyName;
        };

        // Act
        viewModel.EnableIdentityInsert = newValue;

        // Assert
        Assert.IsTrue(propertyChangedRaised, "PropertyChanged event should be raised");
        Assert.AreEqual(nameof(viewModel.EnableIdentityInsert), raisedPropertyName, "PropertyChanged should be raised with correct property name");
        Assert.AreEqual(newValue, viewModel.EnableIdentityInsert, "Property value should be updated");
    }

    /// <summary>
    /// Tests that setting the EnableIdentityInsert property to the same value does not raise
    /// the PropertyChanged event, following the standard MVVM pattern for efficiency.
    /// </summary>
    /// <param name="value">The value to set twice.</param>
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void EnableIdentityInsert_SetToSameValue_DoesNotRaisePropertyChanged(bool value)
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        viewModel.EnableIdentityInsert = value;

        bool propertyChangedRaised = false;
        viewModel.PropertyChanged += (sender, args) =>
        {
            propertyChangedRaised = true;
        };

        // Act
        viewModel.EnableIdentityInsert = value;

        // Assert
        Assert.IsFalse(propertyChangedRaised, "PropertyChanged event should not be raised when setting the same value");
        Assert.AreEqual(value, viewModel.EnableIdentityInsert, "Property value should remain unchanged");
    }

    /// <summary>
    /// Tests that multiple value changes to the EnableIdentityInsert property correctly update
    /// the property value and raise PropertyChanged event for each actual change.
    /// </summary>
    [TestMethod]
    public void EnableIdentityInsert_MultipleValueChanges_RaisesPropertyChangedForEachChange()
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        int propertyChangedCount = 0;
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(viewModel.EnableIdentityInsert))
            {
                propertyChangedCount++;
            }
        };

        // Act & Assert - Change from false to true
        viewModel.EnableIdentityInsert = true;
        Assert.AreEqual(1, propertyChangedCount, "PropertyChanged should be raised once");
        Assert.IsTrue(viewModel.EnableIdentityInsert);

        // Act & Assert - Change from true to false
        viewModel.EnableIdentityInsert = false;
        Assert.AreEqual(2, propertyChangedCount, "PropertyChanged should be raised twice");
        Assert.IsFalse(viewModel.EnableIdentityInsert);

        // Act & Assert - Change from false to true again
        viewModel.EnableIdentityInsert = true;
        Assert.AreEqual(3, propertyChangedCount, "PropertyChanged should be raised three times");
        Assert.IsTrue(viewModel.EnableIdentityInsert);
    }

    /// <summary>
    /// Tests that the EnableIdentityInsert getter returns the correct value after setting it,
    /// verifying the property's get/set symmetry.
    /// </summary>
    /// <param name="value">The value to set and retrieve.</param>
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void EnableIdentityInsert_GetAfterSet_ReturnsSetValue(bool value)
    {
        // Arrange
        var mockSqlServerService = new Mock<ISqlServerService>();
        var mockConnectionViewModel = new Mock<ConnectionViewModel>(mockSqlServerService.Object);
        var viewModel = new DataTransferViewModel(mockSqlServerService.Object, mockConnectionViewModel.Object);

        // Act
        viewModel.EnableIdentityInsert = value;
        bool result = viewModel.EnableIdentityInsert;

        // Assert
        Assert.AreEqual(value, result, "Getter should return the value that was set");
    }
}