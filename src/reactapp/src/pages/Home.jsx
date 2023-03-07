import React from 'react';

function Home() {
    return (
        <>
            <h1>Welcome to Microsoft Assessments</h1>

            <h2 id="introduction">Introduction</h2>
            <p>This application allows you to run assessments against your Microsoft 365 and Entra Azure Active Directory. The assessments checks your tenant configuration and provides recommendations on improving the security posture.</p>

            <h2 id="prerequisites">Pre-Requisites</h2>
            <p>In order to run the assessment, this application requires read-only access to your tenant. Consent will be required from the administrator when first run against a tenant.</p>

            <h2 id="privacy">Security</h2>
            <p>This application runs locally on your device and does not upload and content to any servers.</p>
        </>
    );
}

export default Home;