import React from 'react';
import { makeStyles, typographyStyles } from '@fluentui/react-components';
import { Document, Page } from "react-pdf/dist/esm/entry.webpack";
import aadmindmap from './azureadv1.pdf'

const useStyles = makeStyles({
    text: typographyStyles.title2,
});

function Home() {
    const styles = useStyles();

    return (
        <>
            <span className={styles.text}>Microsoft Entra Mind Map</span>
            <div className="App">
                <a href={aadmindmap} target="_blank">Download Microsoft Entra Mind Map</a>
            </div>

            <iframe src='/assets/mindmaps/azureadv1.pdf#toolbar=0' type="application/pdf" width='300px' height='310px'></iframe>
            {/* <div class="mindMapContainer">
                <iframe src='/assets/mindmaps/azureadv1.pdf' type="application/pdf" ></iframe>
            </div> */}

            {/* <div class="mindMapContainer">
                <Document file={aadmindmap}>
                    <Page pageNumber={1} />
                </Document>
            </div> */}

        </>
    );
}

export default Home;