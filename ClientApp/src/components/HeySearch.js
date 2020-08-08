import React from 'react';
import HeySearchTable from './HeySearchTable';

class HeySearch extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            searchTerm: "",
            isImageOnly: false,
            isOriginalOnly: false,
            nextToken: "",
            data: [],
            message: ""
        };
        this.search = this.search.bind(this);
    }

    /**
     * Make a search request and load the response data. 
     * @param {bool} useNext true to use next page token 
     */
    search(useNext=false) {

        //Construct the target end node query URL
        const xhr = new XMLHttpRequest();
        const url = this.props.url 
            + "?q=" + this.state.searchTerm 
            + (this.state.isImageOnly ? "&io=1" : "") 
            + (this.state.isOriginalOnly ? "&oo=1" : "")
            + (useNext===true && this.state.nextToken!=="" ? "&nt="+this.state.nextToken : "");

        console.log(`EndPoint URL: ${url}`);

        //Request the search result 
        xhr.open('get', url, true);
        xhr.onload = () => {
            if(xhr.status==200) {
                const response = JSON.parse(xhr.responseText);
                if(useNext===true) {
                    //response for next page - append the new data to the existing data. 
                    this.setState({ data: [...this.state.data, ...response.items ] })
                } else {
                    //new response - replace the data
                    this.setState({ data: response.items });
                }
                this.setState({ nextToken: response.nextPageToken });
            } else if(xhr.status==404) {
                if(useNext===true) {
                    // there is no more page clear nextPageToken
                    this.setState({ nextToken: "" });
                } else {
                    this.setState({ data: [] , message: "No result found"})
                }
            } else {
            }
        };
        xhr.send();
    }

    render() {

        // if there is next token, show "Load More" button for the next page
        let loadMore = "";
        if(this.state.nextToken) {
            loadMore = <button onClick={()=>this.search(true)}>Load More</button>;
        }

        // if there is a message to show, display it 
        let message = "";
        if(this.state.message) {
            message = <div><span>{this.state.message}</span></div>;
        }

        return(
            <div>
                <div>
                    <label htmlFor="io">Search words: </label>
                    <input type="text" placeholder="Enter search words" name="search"
                            value={this.state.searchTerm} 
                            onChange={(e)=>this.setState({searchTerm: e.target.value})} />
                    
                    <br />
                    
                    <input type="checkbox" id="io" name="io" value="1" 
                            defaultChecked={this.state.isImageOnly} 
                            onChange={()=>this.setState({isImageOnly: !this.state.isImageOnly})} />
                    <label htmlFor="io">Search tweets with image only</label>
                    
                    <input type="checkbox" id="oo" name="oo" value="1" 
                            defaultChecked={this.state.isOriginalOnly} 
                            onChange={()=>this.setState({isOriginalOnly: !this.state.isOriginalOnly})} />
                    <label htmlFor="oo">Search original (no retweets)</label>
                    
                    <br />

                    <button disabled={(this.state.searchTerm==="")}
                            onClick={()=>this.search()} >Search</button>
                </div>

                {message}

                <HeySearchTable data={this.state.data} />
                
                {loadMore}
            </div>
        );
    }
}

export default HeySearch;