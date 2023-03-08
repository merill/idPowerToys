import React from 'react';
import { CaDocGenTabs } from '../../components/CaDocGenTabs';

function Home() {
    return (
        <>
            <h1>Conditional Access Documenter</h1>
            <p>Export conditional access policies as a PowerPoint presentation.</p>
            <CaDocGenTabs/>
        </>
    );
}

export default Home;