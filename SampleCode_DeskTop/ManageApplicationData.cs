#region DISCLAIMER
/* Copyright (c) 2004-2010 IP Commerce, INC. - All Rights Reserved.
 *
 * This software and documentation is subject to and made
 * available only pursuant to the terms of an executed license
 * agreement, and may be used only in accordance with the terms
 * of said agreement. This software may not, in whole or in part,
 * be copied, photocopied, reproduced, translated, or reduced to
 * any electronic medium or machine-readable form without
 * prior consent, in writing, from IP Commerce, INC.
 *
 * Use, duplication or disclosure by the U.S. Government is subject
 * to restrictions set forth in an executed license agreement
 * and in subparagraph (c)(1) of the Commercial Computer
 * Software-Restricted Rights Clause at FAR 52.227-19; subparagraph
 * (c)(1)(ii) of the Rights in Technical Data and Computer Software
 * clause at DFARS 252.227-7013, subparagraph (d) of the Commercial
 * Computer Software--Licensing clause at NASA FAR supplement
 * 16-52.227-86; or their equivalent.
 *
 * Information in this software is subject to change without notice
 * and does not represent a commitment on the part of IP Commerce.
 * 
 * Sample Code is for reference Only and is intended to be used for educational purposes. It's the responsibility of 
 * the software company to properly integrate into thier solution code that best meets thier production needs. 
*/
#endregion DISCLAIMER

using System;
using System.Configuration;
using System.Windows.Forms;
using schemas.ipcommerce.com.Ipc.General.WCF.Contracts.Common.External.SvcInfo;

namespace SampleCode
{
    public partial class ManageApplicationData : Form
    {
        private ApplicationLocation _ApplicationLocation;
        private HardwareType _HardwareType;
        private PINCapability _PINCapability;
        private ReadCapability _ReadCapability;
        private EncryptionType _EncryptionType;
        private HelperMethods _Helper = new HelperMethods();
        private FaultHandler.FaultHandler _FaultHandler = new FaultHandler.FaultHandler();
        public bool SaveSuccess;

        public ManageApplicationData()
        {
            InitializeComponent();

            //Populate drop downs with enumerated values
            cboApplicationAttended.Items.Add(true);
            cboApplicationAttended.Items.Add(false);

            cboApplicationLocation.Sorted = true;
            cboApplicationLocation.DataSource = Enum.GetValues(typeof(ApplicationLocation));
            cboApplicationLocation.SelectedItem = ApplicationLocation.NotSet;

            CboEncryptionType.Sorted = true;
            CboEncryptionType.DataSource = Enum.GetValues(typeof(EncryptionType));
            CboEncryptionType.SelectedItem = EncryptionType.NotSet;

            cboHardwareType.Sorted = true;
            cboHardwareType.DataSource = Enum.GetValues(typeof(HardwareType));
            cboHardwareType.SelectedItem = HardwareType.NotSet;

            cboPINCapability.Sorted = true;
            cboPINCapability.DataSource = Enum.GetValues(typeof(PINCapability));
            cboPINCapability.SelectedItem = PINCapability.NotSet;

            cboReadCapability.Sorted = true;
            cboReadCapability.DataSource = Enum.GetValues(typeof(ReadCapability));
            cboReadCapability.SelectedItem = ReadCapability.NotSet;
        }

        public void CallingForm(HelperMethods helper)
        {
            _Helper = helper;
        }

        private void ManageApplicationData_Load(object sender, EventArgs e)
        {
            GetApplicationData();
        }

        private void GetApplicationData()
        {
            //Set Values from the calling form
            txtPTLSSocketId.Text = ((SampleCode_DeskTop)(Owner)).PtlsSocketId;

            if (_Helper.ApplicationProfileId.Length < 1) return;
            txtApplicationProfileId.Text = _Helper.ApplicationProfileId.Trim();
            
            //Call GetApplicationData if a previous applicationProfileId exists
            ApplicationData AD = new ApplicationData();
            //From the calling form
            string _strSessionToken = ((SampleCode_DeskTop)(Owner)).Helper.SessionToken;
            try
            {
                ((SampleCode_DeskTop)(Owner)).Helper.CheckTokenExpire();
                AD = ((SampleCode_DeskTop)(Owner)).Helper.Cwssic.GetApplicationData(_strSessionToken, txtApplicationProfileId.Text);
            }
            catch
            {
                MessageBox.Show("Unable to pull application data for persisted ApplicationProfileId in the file '[SK]_applicationProfileId.config'");
                txtApplicationProfileId.Text = "";
                return;
            }
            
            //If an ApplicationData was returned set all of the values.
            cboApplicationAttended.SelectedItem = AD.ApplicationAttended;
            //Select the index that matches
            if (AD.ApplicationAttended | !AD.ApplicationAttended)
            {
                cboApplicationAttended.SelectedItem = AD.ApplicationAttended;
            }
            //Select the index that matches
            if (AD.ApplicationLocation.ToString().Length > 0)
            {
                cboApplicationLocation.SelectedItem = AD.ApplicationLocation;
            }
            txtApplicationName.Text = AD.ApplicationName;
            txtDeveloperId.Text = AD.DeveloperId;
            TxtDeviceSerialNumber.Text = AD.DeviceSerialNumber;
            //Select the index that matches
            if (AD.EncryptionType.ToString().Length > 0)
            {
                CboEncryptionType.SelectedItem = AD.EncryptionType;
            }
            //Select the index that matches
            if (AD.HardwareType.ToString().Length > 0)
            {
                cboHardwareType.SelectedItem = AD.HardwareType;
            }
            //Select the index that matches
            if (AD.PINCapability.ToString().Length > 0)
            {
                cboPINCapability.SelectedItem = AD.PINCapability;
            }
            txtPTLSSocketId.Text = AD.PTLSSocketId;
            //Select the index that matches
            if (AD.ReadCapability.ToString().Length > 0)
            {
                cboReadCapability.SelectedItem = AD.ReadCapability;
            }
            txtSerialNumber.Text = AD.SerialNumber;
            txtSoftwareVersion.Text = AD.SoftwareVersion;
            txtSoftwareVersionDate.Text = AD.SoftwareVersionDate.ToString();
            txtVendorId.Text = AD.VendorId;
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            if (txtApplicationProfileId.Text.Length > 0)
            {
                DialogResult Result;
                Result = MessageBox.Show(
                    "The following will attempt to overwrite an existing ApplicationProfileId. Do you want to continue?",
                    "Overwrite", MessageBoxButtons.OKCancel);
                if (Result == DialogResult.Cancel) return;
            }

            if (!checkRequiredValues()){ MessageBox.Show("You are missing required values");return;}

            try
            {
                ApplicationData AD = new ApplicationData();
                
                //From the calling form
                ((SampleCode_DeskTop)(Owner)).Helper.CheckTokenExpire();
                string _strSessionToken = ((SampleCode_DeskTop)(Owner)).Helper.SessionToken;

                AD.ApplicationAttended = Convert.ToBoolean(cboApplicationAttended.SelectedItem);
                AD.ApplicationLocation = _ApplicationLocation;
                AD.ApplicationName = txtApplicationName.Text;
                AD.DeveloperId = txtDeveloperId.Text;
                AD.DeviceSerialNumber = TxtDeviceSerialNumber.Text;
                AD.EncryptionType = _EncryptionType;
                AD.HardwareType = _HardwareType;
                AD.PINCapability = _PINCapability;
                AD.PTLSSocketId = txtPTLSSocketId.Text.Trim(); //Always remove beginning and end spaces as well as CrLF 
                AD.ReadCapability = _ReadCapability;
                AD.SerialNumber = txtSerialNumber.Text;
                AD.SoftwareVersion = txtSoftwareVersion.Text;
                //ToDo probably need a better way
                AD.SoftwareVersionDate = Convert.ToDateTime(txtSoftwareVersionDate.Text).ToUniversalTime();
                AD.VendorId = txtVendorId.Text;
                
                string strApplicationProfileId = ((SampleCode_DeskTop)(Owner)).Helper.Cwssic.SaveApplicationData(_strSessionToken, AD);
                txtApplicationProfileId.Text = strApplicationProfileId;
                ((SampleCode_DeskTop)(Owner)).Helper.ApplicationProfileId = strApplicationProfileId.Trim();
                MessageBox.Show(
                        "ApplicationData successfully saved. Your application should persist and cache the ApplicationProfileId returned. "
                        + "This ApplicationProfileID will be used for all subsequent transaction processing. \r\n\r\nFor now, the values have been saved in a text file, which is located"
                        + " in the same folder as the executing application '[SK]_applicationProfileId.config'");
                SaveSuccess = true;
                this.Close();    
            }
            catch (Exception ex)
            {
                string strErrorId;
                string strErrorMessage;
                if (_FaultHandler.handleSvcInfoFault(ex, out strErrorId, out strErrorMessage))
                { MessageBox.Show(strErrorId + " : " + strErrorMessage); }
                else { MessageBox.Show(ex.Message); }
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (txtApplicationProfileId.Text.Length < 1) { MessageBox.Show("Please enter a valid ApplicationProfileId in order to delete the ApplicationData"); return; }

            //From the calling form
            ((SampleCode_DeskTop)(Owner)).Helper.CheckTokenExpire();
            string _strSessionToken = ((SampleCode_DeskTop)(Owner)).Helper.SessionToken; 
            try
            {
                ((SampleCode_DeskTop)(Owner)).Helper.Cwssic.DeleteApplicationData(_strSessionToken,
                                                                                       txtApplicationProfileId.Text);
                MessageBox.Show("Successfully deleted " + txtApplicationProfileId.Text);
                Close();
            }
            catch (Exception ex)
            {
                string strErrorId;
                string strErrorMessage;
                if (_FaultHandler.handleSvcInfoFault(ex, out strErrorId, out strErrorMessage))
                { MessageBox.Show(strErrorId + " : " + strErrorMessage); }
                else { MessageBox.Show(ex.Message); }
            }
        }

        private bool checkRequiredValues()
        {
            try
            {
                Convert.ToBoolean(cboApplicationAttended.SelectedItem);
            }
            catch
            {
                return false;
            }
            if(_ApplicationLocation.ToString() == "NotSet")return false;
            if(txtApplicationName.Text.Length == 0)return false;
            //if(txtDeveloperId.Text.Length == 0)return false; //CONDITIONAL SO NO CHECK
            if(_HardwareType.ToString() == "NotSet")return false; 
            if(_PINCapability.ToString() == "NotSet")return false; 
            if(txtPTLSSocketId.Text.Length == 0)return false;
            if (_ReadCapability.ToString() == "NotSet") return false;
            if(txtSerialNumber.Text.Length == 0)return false;
            if(txtSoftwareVersion.Text.Length == 0)return false;
            try
            {
                Convert.ToDateTime(txtSoftwareVersionDate.Text).ToUniversalTime();
            }
            catch
            {
                return false;
            }
            //if (txtVendorId.Text.Length == 0) return false; //CONDITIONAL SO NO CHECK
            
            return true;
        }

        #region Combo Form Events

        private void cboApplicationLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            _ApplicationLocation = (ApplicationLocation)cboApplicationLocation.SelectedItem;
        }

        private void cboHardwareType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _HardwareType = (HardwareType)cboHardwareType.SelectedItem;
        }
        private void CboEncryptionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _EncryptionType = (EncryptionType)CboEncryptionType.SelectedItem;
        }
        private void cboPINCapability_SelectedIndexChanged(object sender, EventArgs e)
        {
            _PINCapability = (PINCapability)cboPINCapability.SelectedItem;
        }

        private void cboReadCapability_SelectedIndexChanged(object sender, EventArgs e)
        {
            _ReadCapability = (ReadCapability)cboReadCapability.SelectedItem;
        }

        #endregion Combo Form Events

        private void cmdClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cmdPopulateTestValues_Click(object sender, EventArgs e)
        {
			//https://my.ipcommerce.com/Docs/TransactionProcessing/CWS/API_Reference/2.0.17/ServiceInfoDataElements/ApplicationData.aspx
            MessageBox.Show(
                "Please note that the following values are generic. Depending on the scope of your integration the following values may change. Please contact your solution consultant with any questions.");
            txtPTLSSocketId.Text = ((SampleCode_DeskTop)(Owner)).PtlsSocketId;
            try { cboApplicationAttended.SelectedItem = Convert.ToBoolean(ConfigurationSettings.AppSettings["ApplicationAttended"]); }
            catch { }
            try { cboApplicationLocation.SelectedItem = (ApplicationLocation)Enum.Parse(typeof(ApplicationLocation), ConfigurationSettings.AppSettings["ApplicationLocation"]); }
            catch { }
            txtApplicationName.Text = "MyTestApp";//
            txtDeveloperId.Text = "TPP123"; //Only used for First Data
            TxtDeviceSerialNumber.Text = "";
            cboHardwareType.SelectedItem = HardwareType.PC;
            try { CboEncryptionType.SelectedItem = (EncryptionType)Enum.Parse(typeof(EncryptionType), ConfigurationSettings.AppSettings["EncryptionType"]); }
            catch { }
            cboHardwareType.SelectedItem = HardwareType.PC;
            try { cboPINCapability.SelectedItem = (PINCapability)Enum.Parse(typeof(PINCapability), ConfigurationSettings.AppSettings["PINCapability"]); }
            catch { }
            try { cboReadCapability.SelectedItem = (ReadCapability)Enum.Parse(typeof(ReadCapability), ConfigurationSettings.AppSettings["ReadCapability"]); }
            catch { }
            txtSerialNumber.Text = "208093707";
            txtSoftwareVersion.Text = "1.0";
            txtSoftwareVersionDate.Text = "2010/05/10";
        }


    }
}