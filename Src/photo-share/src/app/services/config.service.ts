import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Config } from '../models/config';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  static config: Config;

  constructor(private http: HttpClient) { }

  load() {
      const jsonFile = `assets/config/config.${environment.name}.json`;
      return new Promise<void>((resolve, reject) => {
          this.http.get(jsonFile).toPromise().then((response : Config) => {
            ConfigService.config = <Config>response;
            resolve();
          }).catch((response: any) => {
            reject(`Could not load file '${jsonFile}': ${JSON.stringify(response)}`);
          });
      });
  }
}
